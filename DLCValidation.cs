using System.Threading;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace HamsterCombat
{
    public class DLCValidation
    {
        private readonly uint _dlcID;
        private readonly Callback<ValidateAuthTicketResponse_t> _validator;
        private readonly Callback<GetAuthSessionTicketResponse_t> _authTicket;

        private readonly byte[] _ticketData;
        private readonly uint _actualSize;
        private readonly HAuthTicket _ticket;
        
        public bool HasLicense { get; private set; }

        public DLCValidation(uint dlcID)
        {
            if (!SteamManager.Initialized) return;
            
            _dlcID = dlcID;
            _validator = Callback<ValidateAuthTicketResponse_t>.Create(ValidateAuthTicket);
            _authTicket = Callback<GetAuthSessionTicketResponse_t>.Create(AuthTicket);
            
            _ticketData = new byte[1024];
            _ticket = SteamUser.GetAuthSessionTicket(_ticketData, _ticketData.Length, out _actualSize);
        }

        private void AuthTicket(GetAuthSessionTicketResponse_t response)
        {
            Debug.Log(response.m_eResult);
            if (response.m_eResult == EResult.k_EResultOK)
            {
                EBeginAuthSessionResult result = 
                    SteamUser.BeginAuthSession(_ticketData, (int)_actualSize, SteamUser.GetSteamID());
                
                Debug.Log(result);
            }
        }

        public void OnDestroy()
        {
            _validator.Dispose();
            _authTicket.Dispose();
        }

        private async void ValidateAuthTicket(ValidateAuthTicketResponse_t validateAuthTicketResponseT)
        {
            Debug.Log(validateAuthTicketResponseT.m_eAuthSessionResponse);
            if (validateAuthTicketResponseT.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseOK)
            {
                CancellationToken token = AsyncUtils.Instance.GetCancellationToken();
                
                await AsyncUtils.Instance.Wait(1f);

                if (!token.IsCancellationRequested)
                {
                    EUserHasLicenseForAppResult result = SteamUser.UserHasLicenseForApp(
                        SteamUser.GetSteamID(),
                        new AppId_t(_dlcID));

                    Debug.Log(result);
                    if (result == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
                    {
                        HasLicense = true;
                    }
                }
            }
            
            SteamUser.CancelAuthTicket(_ticket);
            SteamUser.EndAuthSession(SteamUser.GetSteamID());
        }
    }
}