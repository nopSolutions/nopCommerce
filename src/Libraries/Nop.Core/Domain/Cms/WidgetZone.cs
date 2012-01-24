
namespace Nop.Core.Domain.Cms
{
    /// <summary>
    /// Represents the widget zone
    /// </summary>
    public enum WidgetZone : int
    {
        //desktop version
        HeadHtmlTag = 10,
        AfterBodyStartHtmlTag = 20,
        HeaderLinks = 30,
        HeaderSelectors = 40,
        HeaderMenu = 50,
        BeforeContent = 60,
        BeforeLeftSideColumn = 70,
        AfterLeftSideColumn = 80,
        BeforeMainColumn = 90,
        AfterMainColumn = 100,
        BeforeRightSideColumn = 110,
        AfterRightSideColumn = 120,
        AfterContent = 130,
        Footer = 140,
        BeforeBodyEndHtmlTag = 150,
        //mobile version
        MobileHeadHtmlTag = 510,
        MobileAfterBodyStartHtmlTag = 520,
        MobileHeaderLinks = 530,
        MobileBeforeContent = 540,
        MobileAfterContent = 550,
        MobileFooter = 560,
        MobileBeforeBodyEndHtmlTag = 570,
    }
}
