﻿using Centaurus.Models;
using stellar_dotnet_sdk.xdr;
using System;
using System.Collections.Generic;
using System.Text;
using static stellar_dotnet_sdk.xdr.OperationType;

namespace Centaurus.Domain
{
    public static class PaymentsHelper
    {
        private static bool TryGetAsset(Asset xdrAsset, out int asset)
        {
            var assetAlphaNum = stellar_dotnet_sdk.Asset.FromXdr(xdrAsset) as stellar_dotnet_sdk.AssetTypeCreditAlphaNum;

            asset = 0;
            string assetSymbol = "XLM";
            if (xdrAsset.Discriminant.InnerValue != AssetType.AssetTypeEnum.ASSET_TYPE_NATIVE)
                assetSymbol = $"{assetAlphaNum.Code}-{assetAlphaNum.Issuer}";

            var assetSettings = Global.Constellation.Assets.Find(a => a.ToString() == assetSymbol);
            if (assetSettings == null) return false;
            asset = assetSettings.Id;
            return true;
        }

        public static bool FromOperationResponse(Operation.OperationBody operation, stellar_dotnet_sdk.KeyPair source, PaymentResults pResult, byte[] transactionHash, out PaymentBase payment)
        {
            payment = null;
            int asset;
            bool result = false;
            switch (operation.Discriminant.InnerValue)
            {
                case OperationTypeEnum.PAYMENT:
                    if (!TryGetAsset(operation.PaymentOp.Asset, out asset))
                        return result;

                    var amount = operation.PaymentOp.Amount.InnerValue;
                    var destKeypair = stellar_dotnet_sdk.KeyPair.FromXdrPublicKey(operation.PaymentOp.Destination.InnerValue);
                    if (Global.Constellation.Vault.Equals((RawPubKey)destKeypair.PublicKey))
                        payment = new Deposit
                        {
                            Destination = new RawPubKey() { Data = source.PublicKey }
                        };
                    else if (Global.Constellation.Vault.Equals((RawPubKey)source.PublicKey))
                    {
                        var withdrawal = Global.WithdrawalStorage.GetWithdrawal(transactionHash);
                        if (withdrawal == null)
                            throw new Exception("Unable to find withdrawal by hash");
                        payment = new Withdrawal
                        {
                            Source = withdrawal.Account
                        };
                    }
                    if (payment != null)
                    {
                        payment.Amount = amount;
                        payment.Asset = asset;
                        payment.TransactionHash = transactionHash;
                        payment.PaymentResult = pResult;

                        result = true;
                    }

                    break;
                case OperationTypeEnum.PATH_PAYMENT:
                    //TODO: handle path payment
                    break;
                case OperationTypeEnum.ACCOUNT_MERGE:
                    //TODO: handle account merge
                    break;
            }
            return result;
        }
    }
}