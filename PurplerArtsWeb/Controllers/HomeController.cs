﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WazeCreditGreen.Models;
using WazeCreditGreen.Service;
using WazeCreditGreen.Models.ViewModels;
using WazeCreditGreen.Utility.AppSettingClasses;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WazeCreditGreen.Data;
using WazeCreditGreen.Data.Repository.IRepository;

namespace WazeCreditGreen.Controllers {
    public class HomeController : Controller {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger) {
        //    _logger = logger;
        //}
        public HomeVM homeVM { get; set; }
        private readonly IMarketForecaster _marketForecaster;
        private readonly ICreditValidator _creditValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TwilioSettings _twilioSettings;
        private readonly StripeSettings _stripeSettings;
        private readonly SendGridSettings _sendGridSettings;
        private readonly WazeForecastingSettings _wazeForecastingSettings;
        private readonly ILogger _logger;
        [BindProperty]
        public CreditApplication CreditModel { get; set; }

        public HomeController(IMarketForecaster marketForecaster,
        IOptions<WazeForecastingSettings> wazeForecastingSettings,
        ICreditValidator creditValidator,
        ILogger<HomeController> logger,
        IUnitOfWork unitOfWork,
        ApplicationDbContext db) {

            homeVM = new HomeVM();
            _logger = logger;
            _wazeForecastingSettings = wazeForecastingSettings.Value;
            _marketForecaster = marketForecaster;
            _creditValidator = creditValidator;
            _unitOfWork = unitOfWork;
            //_db = db;

        }

        public IActionResult AllConfigSettings(
            [FromServices] IOptions<StripeSettings> stripeSettings,
            [FromServices] IOptions<TwilioSettings> twilioSettings,
            [FromServices] IOptions<SendGridSettings> sendGridSettings) {
            List<string> messages = new List<string>();
            messages.Add($"Waze config - Forecast Tracker" + _wazeForecastingSettings.ForecastTrackerEnabled);
            messages.Add($"Stripe Publishable Key: " + stripeSettings.Value.PublishableKey);
            messages.Add($"Stripe Secret Key:" + stripeSettings.Value.SecretKey);
            messages.Add($"Send Grid Key:" + sendGridSettings.Value.SenderGridKey);
            messages.Add($"Twilio Phone: " + twilioSettings.Value.PhoneNumber);
            messages.Add($"Twilio Sid: " + twilioSettings.Value.AccountSid);
            messages.Add($"Twilio Token: " + twilioSettings.Value.AuthToken);
            return View(messages);

        }


        public IActionResult Index() {
            MarketResult currentMarket = _marketForecaster.GetMarketPrediction();
            switch (currentMarket.MarketCondition) {
                case MarketCondition.StableDown:
                    homeVM.MarketForecast = "Market shows signs to go down in a stable state! It is a not a good sign to apply for credit applications! But extra credit is always piece of mind if you have handy when you need it.";
                    break;
                case MarketCondition.StableUp:
                    homeVM.MarketForecast = "Market shows signs to go up in a stable state! It is a great sign to apply for credit applications!";
                    break;
                case MarketCondition.Volatile:
                    homeVM.MarketForecast = "Market shows signs of volatility. In uncertain times, it is good to have credit handy if you need extra funds!";
                    break;
                default:
                    homeVM.MarketForecast = "Apply for a credit card using our application!";
                    break;
            }
            _logger.LogInformation("Home Conteoller Index Action Ended");
            return View(homeVM);
        }

        public IActionResult CreditApplication() {
            CreditModel = new CreditApplication();
            return View(CreditModel);

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("CreditApplication")]
        public async Task<ActionResult> CreditApplicationPost(
            [FromServices] Func<CreditApprovedEnum, ICreditApproved> _creditService) {
            if (ModelState.IsValid) {
                var (validationPassed, errorMessages)
                = await _creditValidator.PassAllValidations(CreditModel);

                CreditResult creditResult = new CreditResult() {
                    ErrorList = errorMessages,
                    CreditID = 0,
                    Success = validationPassed
                };
                if (validationPassed) {
                    CreditModel.CreditApproved = _creditService(
                        CreditModel.Salary > 50000 ?
                        CreditApprovedEnum.High : CreditApprovedEnum.Low
                        ).GetCreditApproved(CreditModel);

                    //add record to database
                    _unitOfWork.CreditApplication.Add(CreditModel);
                    _unitOfWork.Save();
                    creditResult.CreditID = CreditModel.Id;
                    creditResult.CreditApproved = CreditModel.CreditApproved;
                    return RedirectToAction(nameof(CreditResult), creditResult);
                } else {
                    return RedirectToAction(nameof(CreditResult), creditResult);
                }

            };
            return View(CreditModel);
        }
        public IActionResult CreditResult(CreditResult creditResult) {
            return View(creditResult);
        }



        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}