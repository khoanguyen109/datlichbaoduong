using RTE;
using System;
using System.IO;
using System.Web;
using System.Linq;
using Vendare.Error;
using System.Web.Mvc;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using MaintenanceSchedule.Web.Filters;
using MaintenanceSchedule.Library.Utilities;
using MaintenanceSchedule.Web.Areas.Admin.Models;
using MaintenanceSchedule.Core.Common.Configuration;
using MaintenanceSchedule.Core.Queries.Datlichbaoduong;
using MaintenanceSchedule.Core.Commands.Datlichbaoduong;
using ServiceEntity = MaintenanceSchedule.Entity.Datlichbaoduong.Service;
using System.Collections.Generic;

namespace MaintenanceSchedule.Web.Areas.Admin.Controllers
{
    [Authentication]
    public class ServiceController : AdminController
    {
        private readonly IDeleteServiceCommand _deleteServiceCommand;
        private readonly ICreateServiceCommand _createServiceCommand;
        private readonly IUpdateServiceCommand _updateServiceCommand;
        private readonly IUpdateServiceImageCommand _updateServiceImageCommand;

        private readonly IServiceQuery _serviceQuery;
        private readonly IGetServiceListPagingQuery _getServiceListPagingQuery;

        public ServiceController(IUpdateServiceImageCommand updateServiceImageCommand, IDeleteServiceCommand deleteServiceCommand,
                                 ICreateServiceCommand createServiceCommand, IUpdateServiceCommand updateServiceCommand,
                                 IServiceQuery serviceQuery, IGetServiceListPagingQuery getServiceListPagingQuery)
        {
            _createServiceCommand = createServiceCommand;
            _updateServiceCommand = updateServiceCommand;
            _deleteServiceCommand = deleteServiceCommand;
            _updateServiceImageCommand = updateServiceImageCommand;

            _serviceQuery = serviceQuery;
            _getServiceListPagingQuery = getServiceListPagingQuery;
        }

        //[Authorize]
        public ActionResult ServicePage(SearchPageForm searchPageForm)
        {
            try
            {
                return base.Page<GetServiceListPagingRequest, GetServiceListPagingResponse, ServiceViewModel, ServiceEntity>(searchPageForm, _getServiceListPagingQuery, TransformToViewModel);
            }
            catch (Exception ex)
            {
                var detail = new NameValueCollection();
                if (ex.InnerException != null) detail.Add("innerExeption", ex.InnerException.Message);
                new LoggableException(ex, detail);
                return JsonErrorResult(ex.ToString());
            }
        }

        //[Authorize]
        public ActionResult Index()
        {
            ShowDescribesPage(DescribeType.List, Describe.ServiceLabel);
            return View(new ServiceFormModel());
        }

        //[Authorize]
        public ActionResult Create()
        {
            ShowDescribesPage(DescribeType.Create, Describe.ServiceLabel);
            var formModel = new ServiceFormModel
            {
                Content = PrepairEditor("Content", delegate (Editor editor)
                {
                    editor.Width = Unit.Percentage(100);
                    editor.Height = Unit.Pixel(700);
                    if (Request.HttpMethod == "POST")
                        editor.UploadImage += new UploadImageEventHandler(Editor_UploadImage);
                }).Text
            };
            return View(formModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ServiceFormModel model, HttpPostedFileBase[] files)
        {
            int serviceId = 0;
            model.Content = PrepairEditor("Content", delegate (Editor editor) { }, model.Content).Text;

            var response = _createServiceCommand.Invoke(new CreateServiceRequest
            {
                ServiceId = model.Id,
                ServiceTags = model.Tags,
                ServiceTitle = model.Title,
                ServiceImage = model.Image,
                ServiceContent = model.Content,
                ServiceDescrtiption = model.Description
            });

            if (response.Status == CreateServiceStatus.Success)
            {
                serviceId = response.ServiceId;

                if (files != null && files.Length > 0 && files[0] != null)
                {
                    var file = files[0];
                    var extension = Path.GetExtension(file.FileName);
                    model.Image = string.Concat(new string[]
                    {
                        string.Join(Constant.COLON_SYMBOL_DASH.ToString(),
                        new string[]
                        {
                            serviceId.ToString(),
                            Characters.ConvertToUnSign3(model.Title)
                        }),
                        extension
                    });

                    if (!string.IsNullOrEmpty(model.Image))
                    {
                        var responseImage = _updateServiceImageCommand.Invoke(new UpdateServiceImageRequest
                        {
                            ServiceId = serviceId,
                            ServiceImage = model.Image
                        });

                        if (responseImage.Status)
                            SaveImages(file, model.Image);
                        else
                            ModelState.AddModelError("SaveImage", "Cập nhật hình lỗi");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("InsertData", "Tạo dịch vụ lỗi. Liên hệ admin.");
            }

            var errors = ModelState.Values.SelectMany(value => value.Errors).ToList();
            if (errors.Count <= 0)
                ViewBag.Message = "Tạo dịch vụ thành công!";
            
            ShowDescribesPage(DescribeType.Edit, Describe.ServiceLabel);
            return View(model);
        }

        //[Authorize]
        public ActionResult Edit(int id)
        {
            var formModel = new ServiceFormModel();
            var service = _serviceQuery.GetService(id);
            if (service != null)
            {
                formModel = new ServiceFormModel
                {
                    Id = service.Id,
                    Tags = service.Tags,
                    Title = service.Title,
                    Image = service.Image,
                    Description = service.Description,
                    Content = PrepairEditor("Content", delegate (Editor editor)
                    {
                        editor.Width = Unit.Percentage(100);
                        editor.Height = Unit.Pixel(700);
                        if (Request.HttpMethod == "POST")
                            editor.UploadImage += new UploadImageEventHandler(Editor_UploadImage);
                    }, service.Content).Text
                };
            }
            return View(formModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ServiceFormModel model, HttpPostedFileBase[] files)
        {
            var serviceId = model.Id;
            HttpPostedFileBase file = null;

            if (files != null && files.Length > 0 && files[0] != null)
            {
                file = files[0];
                var extension = Path.GetExtension(file.FileName);
                model.Image = string.Concat(new string[]
                {
                        string.Join(Constant.COLON_SYMBOL_DASH.ToString(),
                        new string[]
                        {
                            serviceId.ToString(),
                            Characters.ConvertToUnSign3(model.Title)
                        }),
                        extension
                });
            }

            model.Content = PrepairEditor("Content", delegate (Editor editor) { }, model.Content).Text;

            var response = _updateServiceCommand.Invoke(new UpdateServiceRequest
            {
                ServiceId = serviceId,
                ServiceTags = model.Tags,
                ServiceTitle = model.Title,
                ServiceImage = model.Image,
                ServiceContent = model.Content,
                ServiceDescription = model.Description
            });

            if (response.Status == UpdateServiceStatus.Success)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.Image))
                        SaveImages(file, model.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("SaveImage", ex.Message.ToString());
                }
            }
            else
            {
                ModelState.AddModelError("SaveData", "Cập nhật dịch vụ lỗi. Liên hệ admin.");
            }

            var errors = ModelState.Values.SelectMany(value => value.Errors).ToList();
            if (errors.Count <= 0)
                ViewBag.Message = "Cập nhật dịch vụ thành công!";
            
            ShowDescribesPage(DescribeType.Edit, Describe.ServiceLabel);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _deleteServiceCommand.Invoke(new DeleteServiceRequest { ServiceId = id });
            if (response.Status)
            {
                //Delete images on disk
                var realImageFile = !string.IsNullOrEmpty(response.Image) ? Server.MapPath(ConfigVariable.RelativeServiceUrl + response.Image) : string.Empty;
                var mediumImageFile = !string.IsNullOrEmpty(response.Image) ? Server.MapPath(ConfigVariable.RelativeServiceMediumUrl + response.Image) : string.Empty;
                var thumbImageFile = !string.IsNullOrEmpty(response.Image) ? Server.MapPath(ConfigVariable.RelativeServiceThumbUrl + response.Image) : string.Empty;

                if (Directory.Exists(realImageFile))
                    Directory.Delete(realImageFile);

                if (Directory.Exists(mediumImageFile))
                    Directory.Delete(mediumImageFile);

                if (Directory.Exists(thumbImageFile))
                    Directory.Delete(thumbImageFile);

                return JsonResult(response.Status, "Đã xóa dịch vụ", false);
            }
            return JsonResult(response.Status, "Xóa dịch vụ lỗi " + response.Exception, false);
        }

        //this action is specified by editor.AjaxPostbackUrl = Url.Action("EditorAjaxHandler");
        //it will handle the editor dialogs Upload/Ajax requests
        [ValidateInput(false)]
        public ActionResult EditorAjaxHandler()
        {
            PrepairEditor("Content", delegate (Editor editor) { });
            return new EmptyResult();
        }

        protected Editor PrepairEditor(string propertyName, Action<Editor> oninit, string formData = "")
        {
            Editor editor = new Editor(System.Web.HttpContext.Current, propertyName);
            editor.ClientFolder = "/Assets/richtexteditor/";

            editor.AjaxPostbackUrl = Url.Action("EditorAjaxHandler");

            if (oninit != null) oninit(editor);

            //try to handle the upload/ajax requests
            bool isajax = editor.MvcInit();

            if (isajax) return editor;

            editor.LoadFormData(formData);
            //render the editor to ViewBag.Editor
            ViewBag.Editor = editor.MvcGetString();

            return editor;
        }

        void Editor_UploadImage(object sender, UploadImageEventArgs args)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(400, 200, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                System.Drawing.FontStyle style = System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline;
                System.Drawing.Font font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace, 26, style);
                System.Drawing.SizeF size = g.MeasureString("", font);
                g.DrawString("", font, System.Drawing.Brushes.DarkGreen, bitmap.Width - size.Width, bitmap.Height - size.Height);
            }

            RTE.ConfigWatermark watermark = new ConfigWatermark();
            watermark.XAlign = "right";
            watermark.YAlign = "bottom";
            watermark.XOffset = -10;
            watermark.YOffset = -10;
            watermark.MinWidth = 450;
            watermark.MinHeight = 300;
            watermark.Image = bitmap;

            args.Watermarks.Clear();
            args.Watermarks.Add(watermark);
            args.DrawWatermarks = true;
        }

        private bool SaveImages(HttpPostedFileBase file, string fileImage)
        {
            //save real size
            var realImageFile = ConfigVariable.ServiceDirectoryUrl + fileImage;
            file.SaveAs(realImageFile);
            Bitmap image = new Bitmap(realImageFile);
            //save medium size
            Images.Instance.SaveAndResizeMedium(image, ConfigVariable.RelativeServiceMediumUrl + fileImage);
            //save thumb size
            Images.Instance.SaveAndResizeSmall(image, ConfigVariable.RelativeServiceThumbUrl + fileImage);
            return true;
        }

        private ServiceViewModel TransformToViewModel(ServiceEntity service)
        {
            return new ServiceViewModel
            {
                Id = service.Id,
                Title = service.Title,
                DisplayImage = "<img alt='" + service.Title + "' src='" + ConfigVariable.RelativeServiceThumbUrl + service.Image + "' />"
            };
        }
    }
}