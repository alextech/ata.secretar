using System;
using Microsoft.JSInterop;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.KYC;

namespace KycViewer.Shared
{
    public class KycDocumentJsBridge
    {
        private readonly KycDocument _document;

        public KycDocumentJsBridge(KycDocument document)
        {
            _document = document;
        }

        [JSInvokable]
        public void SaveNote(string noteName, string note, string section, string sectionId)
        {
            Guid id = Guid.Parse(sectionId);

            switch (section)
            {
                case "client":

                    PClient pClient = _document.GetClientById(id);
                    switch (noteName)
                    {
                        case "income_notes":

                            pClient.Income.Notes = note;
                            break;
                        case "networth_notes":

                            pClient.NetWorth.Notes = note;
                            break;
                        case "knowledge_notes":

                            pClient.Knowledge.Notes = note;
                            break;
                    }

                    break;
                case "document":

                    _document.Notes[noteName] = note;
                    break;
            }
        }
    }
}