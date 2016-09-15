using System;
using System.Drawing;
using System.IO;
using System.Web;
using Vendare.Error;

namespace Vendare.Utils
{
	/// <summary>
	/// Summary description for SizedImage.
	/// </summary>
	public struct SizedImage
	{
		private short width;
		private short height;
		private String path;
		private HttpPostedFile file;
		private short resizeWidth;
		private short resizeHeight;
		private String uploadPath;

		private static SingleError[] possibleErrors;

		private enum ErrorTypes
		{
			BadFile
		}

		static SizedImage() 
		{
			possibleErrors = new SingleError[1];
			possibleErrors[0] = new SingleError((long)ErrorTypes.BadFile, "BadFile", "You may only upload Image files.");
		}

		public SizedImage(short width, short height, String path)
		{
			this.width = width;
			this.height = height;
			this.path = path;

			this.file = null;
			this.resizeWidth = 0;
			this.resizeHeight = 0;
			this.uploadPath = "";

		}

		public SizedImage(HttpPostedFile file, short width, short height, String directory) 
		{
			path = "";
			this.width = 0;
			this.height = 0;

			this.file = file;
			this.resizeWidth = width;
			this.resizeHeight = height;
			this.uploadPath = directory;
		}

		public bool IsValid(ErrorCollection errors) 
		{
			if(file == null || file.ContentLength == 0)
				return true;

			else if(!file.ContentType.StartsWith("image/"))
				errors.Add(possibleErrors[(int)ErrorTypes.BadFile]);

			return errors.IsEmpty;
		}

		public void SaveUpload() 
		{
			if(file==null || file.ContentLength == 0)
				return;

			Image image = Image.FromStream(file.InputStream);

			short origWidth = (short)image.Width;
			short origHeight = (short)image.Height;

			if(resizeWidth > -1 && resizeHeight > -1) 
			{
				double scaleFactor;

				if(origWidth > origHeight) 
				{
					if(origWidth > resizeWidth) 
					{
						scaleFactor = (double)resizeWidth/(double)origWidth;
						origWidth = resizeWidth;
						origHeight = (short)(origHeight * scaleFactor);
					}
				}
				else 
				{
					if(origHeight > resizeHeight) 
					{
						scaleFactor = resizeHeight/(double)origHeight;
						origHeight = resizeHeight;
						origWidth = (short)(origWidth * scaleFactor);
					}
				}
			}

			Bitmap bm = new Bitmap(image, origWidth, origHeight);
			String name = DateTime.Now.Ticks.ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
			String abPath = HttpContext.Current.Request.MapPath(uploadPath) +"\\" + name;
			this.path = uploadPath + "/" + name;
			bm.Save(abPath, image.RawFormat);
			bm.Dispose();
			image.Dispose();
			this .width = origWidth;
			this.height = origHeight;
		}

		public string Path 
		{
			get { return path;}
		}

		public HttpPostedFile UploadedFile 
		{
			get { return file;}
		}

		public short Width 
		{
			get { return width; }
		}

		public short Height 
		{
			get { return height; }
		}

		public void Delete() 
		{
			if(path.Equals(""))
				return;

			String abPath = HttpContext.Current.Request.MapPath(path);
			File.Delete(abPath);

			path = "";
		}

	}
}
