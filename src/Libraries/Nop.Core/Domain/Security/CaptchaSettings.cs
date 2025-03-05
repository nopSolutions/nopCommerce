﻿using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security;

/// <summary>
/// CAPTCHA settings
/// </summary>
public partial class CaptchaSettings : ISettings
{
    /// <summary>
    /// Is CAPTCHA enabled?
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Type of reCAPTCHA
    /// </summary>
    public CaptchaType CaptchaType { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the login page
    /// </summary>
    public bool ShowOnLoginPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the registration page
    /// </summary>
    public bool ShowOnRegistrationPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the contacts page
    /// </summary>
    public bool ShowOnContactUsPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the wishlist page
    /// </summary>
    public bool ShowOnEmailWishlistToFriendPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "email a friend" page
    /// </summary>
    public bool ShowOnEmailProductToFriendPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "comment blog" page
    /// </summary>
    public bool ShowOnBlogCommentPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "comment news" page
    /// </summary>
    public bool ShowOnNewsCommentPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "News letter" page
    /// </summary>
    public bool ShowOnNewsletterPage { get; set; }        

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the product reviews page
    /// </summary>
    public bool ShowOnProductReviewPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "Apply for vendor account" page
    /// </summary>
    public bool ShowOnApplyVendorPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "forgot password" page
    /// </summary>
    public bool ShowOnForgotPasswordPage { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the Forum
    /// </summary>
    public bool ShowOnForum { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the checkout page for guest customers
    /// </summary>
    public bool ShowOnCheckoutPageForGuests { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the check gift card balance page
    /// </summary>
    public bool ShowOnCheckGiftCardBalance { get; set; }

    /// <summary>
    /// The base reCAPTCHA API URL
    /// </summary>
    public string ReCaptchaApiUrl { get; set; }
    /// <summary>
    /// reCAPTCHA public key
    /// </summary>
    public string ReCaptchaPublicKey { get; set; }

    /// <summary>
    /// reCAPTCHA private key
    /// </summary>
    public string ReCaptchaPrivateKey { get; set; }

    /// <summary>
    /// reCAPTCHA V3 score threshold
    /// </summary>
    public decimal ReCaptchaV3ScoreThreshold { get; set; }

    /// <summary>
    /// reCAPTCHA theme
    /// </summary>
    public string ReCaptchaTheme { get; set; }

    /// <summary>
    /// The length of time, in seconds, before the reCAPTCHA request times out
    /// </summary>
    public int? ReCaptchaRequestTimeout { get; set; }
    /// <summary>
    /// reCAPTCHA default language
    /// </summary>
    public string ReCaptchaDefaultLanguage { get; set; }

    /// <summary>
    /// A value indicating whether reCAPTCHA language should be set automatically
    /// </summary>
    public bool AutomaticallyChooseLanguage { get; set; }
}