using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class IsamGiftCardService : IIsamGiftCardService
    {
        private readonly string _tableName = "DSK_GIFT_MAIN";
        private IList<string> _columns;
        private readonly string KEY_ACCT_NUMBER = "KEY_ACCT_NUMBER";
        private readonly string ACCT_NUMBER = "ACCT_NUMBER";
        private readonly string CID_NUMBER = "CID_NUMBER";
        private readonly string GIFT_AMT = "GIFT_AMT";
        private readonly string GIFT_AMT_USED = "GIFT_AMT_USED";
        private IBaseService _baseIsamService;

        Dictionary<string, OdbcParameter> _colToParam = new Dictionary<string, OdbcParameter>();

        public IsamGiftCardService(
            IBaseService baseService
        )
        {
            _baseIsamService = baseService;
            _columns = new List<string> { KEY_ACCT_NUMBER, ACCT_NUMBER, CID_NUMBER, GIFT_AMT, GIFT_AMT_USED };

            _colToParam.Add(KEY_ACCT_NUMBER, new OdbcParameter("@" + KEY_ACCT_NUMBER, OdbcType.VarChar));
            _colToParam.Add(ACCT_NUMBER, new OdbcParameter("@" + ACCT_NUMBER, OdbcType.VarChar));
            _colToParam.Add(CID_NUMBER, new OdbcParameter("@" + CID_NUMBER, OdbcType.VarChar));
            _colToParam.Add(GIFT_AMT, new OdbcParameter("@" + GIFT_AMT, OdbcType.Decimal));
            _colToParam.Add(GIFT_AMT_USED, new OdbcParameter("@" + GIFT_AMT_USED, OdbcType.Decimal));
        }

        public GiftCardInfo GetGiftCardInfo(string code)
        {
            if (code.Length < 4 || code.Length > 18)
            {
                return new GiftCardInfo { GiftCard = null };
            }

            string cid = code.Substring(0, 3);
            string keyNum = code.Substring(3);
            _colToParam[CID_NUMBER].Value = cid;
            _colToParam[KEY_ACCT_NUMBER].Value = keyNum;

            IList<OdbcParameter> argParams = new List<OdbcParameter> { _colToParam[KEY_ACCT_NUMBER], _colToParam[CID_NUMBER] };
            string arg = " WHERE " + KEY_ACCT_NUMBER + " = ? AND " + CID_NUMBER + " = ?;";
            // use base service to get the thing
            DataSet dataSet = _baseIsamService.Get(_tableName, _columns, arg, argParams);

            // builds gift card from information in the dataset
            // grab first row's objects
            DataRow[] rows = dataSet.Tables[0].Select();
            if (rows.Length == 0)
            {
                return new GiftCardInfo { GiftCard = null };
            }

            List<object> row = rows[0].ItemArray.ToList();
            string cardCode = (string)row[0];
            decimal cardTotal = decimal.Parse(row[3].ToString());
            decimal cardUsed = decimal.Parse(row[4].ToString());
            GiftCard giftCard = new GiftCard
            {
                GiftCardCouponCode = cid + cardCode,
                Amount = cardTotal - cardUsed,
                CreatedOnUtc = DateTime.Now,
                IsGiftCardActivated = true
            };

            return new GiftCardInfo { GiftCard = giftCard, AmountUsed = cardUsed };
        }

        // how will i figure out amtUsed??
        public void UpdateGiftCardAmt(GiftCard giftCard, decimal amtUsed)
        {
            string cid = giftCard.GiftCardCouponCode.Substring(0, 3);
            string keyNum = giftCard.GiftCardCouponCode.Substring(3);
            _colToParam[KEY_ACCT_NUMBER].Value = keyNum;
            _colToParam[ACCT_NUMBER].Value = keyNum;
            _colToParam[GIFT_AMT_USED].Value = amtUsed;
            List<OdbcParameter> parameters = new List<OdbcParameter> { _colToParam[GIFT_AMT_USED] };
            List<string> columns = new List<string> { GIFT_AMT_USED };

            string arg = " WHERE " + KEY_ACCT_NUMBER + " = ?;";
            List<OdbcParameter> argParams = new List<OdbcParameter> { _colToParam[KEY_ACCT_NUMBER] };

            _baseIsamService.Update(_tableName, new List<string> { GIFT_AMT_USED }, parameters, arg, argParams);
        }
    }
}
