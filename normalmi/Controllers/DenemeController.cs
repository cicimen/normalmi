﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Authentication;

namespace normalmi.Controllers
{
    public class DenemeController : Controller
    {
        //
        // GET: /Deneme/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Deneme()
        {
            return View();
        }

        public ActionResult SaveUser()
        {
            return View();
        }
	}
}