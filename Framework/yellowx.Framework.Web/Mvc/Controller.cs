using System.Web.Mvc;
using yellowx.Framework.Globalization;

namespace yellowx.Framework.Web.Mvc
{
    public abstract class Controller : System.Web.Mvc.Controller
    {
        public ILocalizerProvider Localizer
        {
            get
            {
                return Configuration.LocalizerProvider;
            }
        }

        #region Localizations
        protected string Localize(string key)
        {
            var countryCode = Request["countryCode"] ?? "vn";
            return Localizer.GetString(countryCode, key);
        }

        #endregion
    }
    public abstract class Controller<TModel> : Controller
    {


        public virtual ActionResult Create()
        {
            var model = default(TModel);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Create(TModel model)
        {
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            var model = default(TModel);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Edit(TModel model)
        {
            return View(model);
        }
    }
}
