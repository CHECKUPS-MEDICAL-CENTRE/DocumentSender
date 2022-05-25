using AfricasTalkingCS;
using DocumentSender.Models.Lab;
using DocumentSender.Models.ViewModels;
using DocumentSender.Services;
using DocumentSender.Services.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QRCoder;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentSender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceProvider;
        //private readonly IDocumentingService _documentingService;
        const string username = "CHECKUPS"; // substitute with your username if mot using sandbox
        const string apikey = "680045bd234623a6b777e1ef1c5c304bd3add14b76c5661e0b6c1c1b9133aea4";
        //const string apikey = "eca849d34c4fa0a6cdd4f0fccc3e3975840eca257c00696c1bbfddeae013ec23"; // substitute with your production API key if not using sandbox

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            // _documentingService = documentingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var ct = _serviceProvider.CreateScope();
            
            //order detail
            var gs = ct.ServiceProvider.GetRequiredService<IGeneralService>();
            var lb = ct.ServiceProvider.GetRequiredService<ILabDocumentsService>();


            while (!stoppingToken.IsCancellationRequested)
            {
                var unsent = await gs.GetUnsentDocuments();
                foreach (var document in unsent)
                {
                    //if (document.DocumentType == "invoice")
                    //{
                    //    await GenerateInvoice(document.CycleId);
                    //    await gs.UpdateSentCertificate(new object[] { document.CycleId, document.DocumentType });
                    //}
                        
                    if (document.DocumentType == "covid_cert")
                    {
                       // GenerateQrCode(document.CycleId);
                        try
                        {
                            //fetch test type
                            var tests= await lb.TestsDone(new object[] { document.CycleId });
                            foreach(var testdone in tests)
                            {
                                //await GenerateCovidCertificate(document.CycleId, testdone.Test);
                              //  await GenerateMemberFinancialReport("CHIN00000054", document.CycleId);

                               // await gs.UpdateSentCertificate(new object[] { document.CycleId, document.DocumentType });
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            await gs.SendToErrorLog(new object[] { ex.StackTrace });
                            _logger.LogError(ex.Message);
                        }

                    }
                    else if(document.DocumentType== "Subscription Model")
                    {
                        await GenerateMemberFinancialReport("CHIN00000054", "167810");
                        await gs.UpdateSentCertificate(new object[] { document.CycleId, document.DocumentType });
                    }
                    else
                    {
                        await gs.UpdateSentCertificate(new object[] { document.CycleId, document.DocumentType });
                    }
                   

                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task GenerateCovidCertificate(string cycle_id, int testtype)
        {
            using var ct = _serviceProvider.CreateScope();
            var lab = ct.ServiceProvider.GetRequiredService<ILabDocumentsService>();
            var gen = ct.ServiceProvider.GetRequiredService<IGeneralService>();
            try
            {
                
                //order detail
               

                //get test details
                var certDetails = await lab.FetchLabCertDetails(new object[] { cycle_id });
                var testDetails = new List<LabTestParticulars>();
                if (testtype == 3530)
                {
                     testDetails = await lab.FecthLabTestParticularsPCR(new object[] { cycle_id });
                }
                else if(testtype == 3558)
                {
                     testDetails = await lab.FecthLabTestParticularsAntigen(new object[] { cycle_id });
                }
                
                LabCertsVM labCerts = new LabCertsVM();
                labCerts.Age = certDetails.Age;
                labCerts.Gender = certDetails.Gender;
                labCerts.IdNumber = certDetails.IdNumber;
                labCerts.Investigation = certDetails.Investigation;
                labCerts.InvoiceNumber = certDetails.InvoiceNumber;
                labCerts.LabSerial = certDetails.LabSerial;
                labCerts.PatientName = certDetails.PatientName;
                labCerts.PatientNumber = certDetails.PatientNumber;
                labCerts.Sample = certDetails.Sample;
                labCerts.TestValue = certDetails.TestValue;
                labCerts.VisitDate = certDetails.VisitDate;
                labCerts.labTestParticulars = testDetails;
                labCerts.SampleCollectionTime = certDetails.SampleCollectionTime;
                labCerts.DOB = certDetails.DOB;


                //pdf generator
                HtmlToPdf converter = new HtmlToPdf();
                //// set converter options
                converter.Options.PdfPageSize = PdfPageSize.A4;

                // header settings
                converter.Options.DisplayHeader = true;
                converter.Header.DisplayOnFirstPage = true;
                converter.Header.DisplayOnOddPages = true;
                converter.Header.DisplayOnEvenPages = true;
                converter.Header.Height = 128;
                converter.Options.DisplayFooter = true;
                converter.Footer.DisplayOnFirstPage = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.Height = 128;

                string htmlString = "";
                htmlString += @$"<!DOCTYPE html>
<html>
<head>
    <link href='C:\Certificate\Res\style.css' rel='stylesheet'>
    <link href='C:\Certificate\Res\bootsrap.min.css' rel='stylesheet' type='text/css'>
    <link href='C:\Certificate\Res\Receipt.css' rel='stylesheet' type='text/css'>    
</head>
<body>
     <div class='container'>
        <div class='panel'>
            <div class='row'>
            </div><br />
            <div style='margin: 20px;'>
                    <div style='color:Black' class='col-md-12 col-sm-12 panelPart'>
                        <h3><b>LABORATORY REPORT</b></h3>
                    </div>
                    <hr style='width:100%;border: none; border-bottom: 2px solid black;' />
            </div>
            <div style='margin: 20px;'>
                <div class='row'>
                    <div class='col-md-5'>
<div class='col-md-12'style='text-align: left;'>
<table>
                        <tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Patient NO#&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.PatientNumber}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Invoice NO:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.InvoiceNumber}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Name:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.PatientName}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Gender:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.Gender}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>DOB:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.DOB.ToString("dd-MM-yyyy")} </small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800;color:black'>ID/Passport:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.IdNumber}</small></td>
                        </tr>
</tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800;color:black'>SAMPLE:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.Sample}</small></td>
                        </tr>
                    </table>
</div>
                    </div>

                <div class='col-md-5'>
<div class='col-md-12'style='text-align: left;'>
<table>
                        <tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Lab SN&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.LabSerial}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Visit Date:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.VisitDate.ToString("dd-MM-yyyy")}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Prescribed By:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;Dr. Grace Wamaitha</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Date Requested:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.VisitDate.ToString("dd-MM-yyyy")}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800; color:black'>Collection Date:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{labCerts.SampleCollectionTime}</small></td>
                        </tr>
<tr>
                            <td style='width:40%;'><small style='font-weight:800;color:black'>Report Date:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small>:&nbsp;&nbsp;&nbsp;{DateTime.UtcNow.AddHours(3)}</small></td>
                        </tr>
                    </table>
</div>
                    </div>
                       <div class='col-md-2'>
                                        <div class='float-left'>
                    <img src='C:\QR\{cycle_id}.png' style='width:100%;'>
                      </div> 

                       </div>
                                   <div class='col-md-12'>
                                        <div class='float-left'>
                    <div class='col-md-2'style='text-align: left;'>
                       <div><b>Investigation</b></div>
                    </div>
                   
                    <div class='col-md-9'style='text-align: left; margin-left: -40px;'>
                        <div>: {labCerts.Investigation}</div>
                    </div>
                      </div> 

                       </div>
                </div>
                  </div>
                      <div style='margin: 20px;'>
                   
               ";
                if (labCerts.TestValue.ToUpper().Contains("POSITIVE"))
                {
                    htmlString += @" <div class='row'>
                   <div class='col-md-12'>
                    <div style='border-color:#c6d9f0;border-style: solid;order-width: thin;border-radius:25px; color:Black' class='col-md-12 col-sm-12 panelPart'>
                        <div class='col-md-4'>
                          <h4>Analyte</h3>
                      </div>
                        <div class='col-md-4'>
                           <h4>Result</h3> 
                        </div>
                       <div class='col-md-4'>
                           <h4>CT Value</h3> 
                        </div>
                    </div>
                   
                ";
                    foreach (var test in testDetails)
                    {

                        if (test.Value.Contains(":"))
                        {
                            string result = test.Value.Split(":")[0];
                            string ctValue = test.Value.Split(":")[1];
                            htmlString += @$" 
                      <div class='col-md-4'>
                          <h5>{test.Test}</h5>
                      </div>

                        <div class='col-md-4'>
                           <h5>{result}</h5> 
                        </div>
                        <div class='col-md-4'>
                           <h5>{ctValue}</h5> 
                      </div> 
                        ";
                        }
                        else
                        {
                            htmlString += @$" <div class='col-md-4'>
                          <h5>{test.Test}</h5>
                      </div>

                        <div class='col-md-4'>
                           <h5>{test.Value}</h5> 
                        </div>
                        <div class='col-md-4'>
                           <h5>.</h5> 
                      </div> 
                        ";
                        }
                    }
                }
                else
                {
                    htmlString += @" <div class='row'>
                   <div class='col-md-12'>
                    <div style='border-color:#c6d9f0;border-style: solid;order-width: thin;border-radius:25px; color:Black' class='col-md-12 col-sm-12 panelPart'>
                        <div class='col-md-6'>
                          <h4>Test</h3>
                      </div>
                        <div class='col-md-6'>
                           <h4>Result</h3> 
                        </div>
                    </div>
                ";
                    foreach (var test in testDetails)
                    {


                        htmlString += @$" <div class='col-md-6'>
                          <h5>{test.Test}</h5>
                      </div>

                        <div class='col-md-6'>
                           <h5>{test.Value}</h5> 
                        </div>
                        ";

                    }
                }

                htmlString += @$" <div style='margin-top: 20px;'>
                   <p style='margin: 0 !important;'><FONT FACE='Trebuchet MS, serif'><b>Review Notes</b></FONT></p>
                      <ul>";
                if (labCerts.Investigation.Contains("ANTIGEN"))
                {
                    htmlString += @$"
<ul>
<li style='margin-bottom: 10px;'>This sample tested {testDetails.FirstOrDefault().Test} for SARS-COV-2 Lateral Flow Rapid Antigen</li>
<li style='margin-bottom: 10px;'>Checkups Medical Centre Molecular Lab uses SARS-CoV-2 rapid antigen test in detection of SARS-CoV-2 highly specific RNA.</li>
<li style='margin-bottom: 10px;'>Negative results do not preclude SARS-COV-2 infection. False negative results may be encountered in samples collected too early or too late in the clinical course of the infection. A repeat test is recommended if clinically or epidemiologically indicated</li>
<li style='margin-bottom: 10px;'>According to the MOH guidelines, all suspected cases testing negative may be subjected to further testing and clinical evaluation for SARS-COV-2 if necessary.</li>
</ul>

";

                }
                else
                {
                    htmlString += @$"
<ul>
<li style='margin-bottom: 10px;'>This sample tested {labCerts.TestValue.Substring(0, 8)} for SARS-COV-2 PCR. Positive and Negative Controls Passed.</li>
<li style='margin-bottom: 10px;'>This test was performed using Bio-Rad CFX-96 RT-PCR. The Novel Coronavirus (SARS-CoV-2) Fast Nucleic Acid Detection Kit (PCR-Fluorescence Probing) is a real-time reverse transcription-polymerase chain reaction (RT -PCR) test designed to detect specific ORF1ab and N genes from SARS-CoV-2 in oropharyngeal swabs (CE Registration NL-CA002-2020-49899; FDA Registration 3016754982). </li>
<li style='margin-bottom: 10px;'>Negative results do not preclude SARS-COV-2 infection. False negative results may be encountered in samples collected too early or too late in the clinical course of the infection. A repeat test is recommended if clinically or epidemiologically indicated</li>
<li style='margin-bottom: 10px;'>According to the MOH guidelines, all suspected cases testing negative may be subjected to further testing and clinical evaluation for SARS-COV-2 if necessary.</li>
</ul>
";
                }
                htmlString += @$"  
                  </div>
<div style='margin-top: 20px;'>
<p style='margin: 0 !important;'><FONT FACE='Trebuchet MS, serif'><b>NB: </b></FONT>If you are a traveler, you will receive your TT CODE from PANABIOS once your COVID-19 PCR report is ready. If not received kindly call +254111050290 or WhatsApp +254115603423 our 24hrs service line.</p>
</div>
                 <div class='row' style='margin-top:70px;'>
<div class='col-md-4'>
      <div class='row' style='color:black'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td style='width:40%;'><small>Tested by:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small style='font-weight:bold; font-style:italic; color:black'>&nbsp;&nbsp;&nbsp;Robert Cheruiyot Ronoh</small></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class='row' style='color:black;'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td><small>Signature:&nbsp;&nbsp;&nbsp;&nbsp;</small></td>
                            <td class='under' style='width:100%'><small style='font-weight:bold; font-style:italic; color:black'>&nbsp;&nbsp;&nbsp;<img src='C:/Certificate/Res/Rono.png'></small></td>
                        </tr>
                    </table>
                </div>
            </div><br />

        </div>
        <div class='col-md-4' style='margin: 0 !important;'>
                       <div style='position: relative;text-align: center;margin-top: -30px !important;'>
  <img src='C:/Certificate/Res/stamp.png'style='width:100%;'>
  <div style='position: absolute;top: 50%;left: 50%;transform: translate(-50%, -50%);'>{DateTime.UtcNow.ToString("dd-MM-yyyy")}</div>
</div>
          </div>
        <div class='col-md-4'>
      <div class='row' style='color:black'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td style='width:40%'><small>Reviewed By:&nbsp;&nbsp;</small></td>
                            <td style='width:100%'><small style='font-weight:bold; font-style:italic; color:black'>&nbsp;&nbsp;&nbsp;Idris Maloba</small></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class='row' style='color:black;'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td ><small>Signature:&nbsp;&nbsp;&nbsp;&nbsp;</small></td>
                            <td class='under' style='width:100%'><small style='font-weight:bold; font-style:italic; color:black'>&nbsp;&nbsp;&nbsp;<img src='C:/Certificate/Res/Idris.png'></small></td>
                        </tr>
                    </table>
                </div>
            </div><br />

        </div>
     </div>
            </div>
             <div class='row'>
</div>
         </div>
         </div>

</div>
</body>
</html>

";

                string headerImage = @"C:\Certificate\Res\header.png";
                PdfImageSection headerImg = new PdfImageSection(0, 0, 594, headerImage);
                converter.Header.Add(headerImg);

                string imgFile = @"C:\Certificate\Res\footer.png";
                PdfImageSection img = new PdfImageSection(0, 0, 594, imgFile);
                converter.Footer.Add(img);


                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                PdfDocument doc1 = converter.ConvertHtmlString(htmlString);
                // save pdf document

                string fileLocation = @$"E:/Certificates/OneDrive/CovidCerts/AllCerts/{cycle_id}.pdf";
                doc.Save(fileLocation);
                // create memory stream to save PDF
                MemoryStream pdfStream = new MemoryStream();
                doc1.Save(pdfStream);
                pdfStream.Position = 0;
                //fetch email
                var email = await gen.GetEmailPhone(new object[] { cycle_id });
                string body = populateCovidEmailBody(email.Name);

                var gateway = new AfricasTalkingGateway(username, apikey);
                var phoneRegex = new Regex("^[a-zA-Z0-9 +]*$");
                string phoneto = email.Phone;
                string message = @$"Dear {email.Name}, Thank you for choosing Checkups Medical centre for your Covid-19 test. Your report is ready and sent on registered email. For any assistance please call 0111050290";
                if (!labCerts.TestValue.ToLower().Contains("positive"))
                {
                    if (phoneRegex.IsMatch(phoneto))
                    {
                        string custPhone = phoneto.StartsWith('0') ? "+254" + phoneto.Substring(1, 9) : phoneto.StartsWith("7") ? "+254" + phoneto : phoneto.StartsWith("254") ? "+" + phoneto : phoneto;
                        try
                        {
                            var sms = gateway.SendMessage(custPhone, message, "CHECKUPS");
                        }
                        catch (AfricasTalkingGatewayException exception)
                        {

                        }

                        //await rd.UpdateSMSStatus(new object[] { recipient.id });
                    }
                }
                else
                {
                    try
                    {
                        string mes = @$"{email.Name} is positive. check your email for certificate and contact patient";
                        var sms = gateway.SendMessage("+254710226738", mes, "CHECKUPS");
                        var sms2 = gateway.SendMessage("+254705867607", mes, "CHECKUPS");
                    }
                    catch (AfricasTalkingGatewayException exception)
                    {

                    }
                }

                EmailService emailService = new EmailService();
                if (email != null)
                {
                    if (!string.IsNullOrWhiteSpace(email.Email) && !string.IsNullOrWhiteSpace(email.Email2))
                    {
                        if (labCerts.TestValue.ToLower().Contains("positive"))
                        {
                            emailService.SendMessage2($"idris.maloba@checkupsmed.com", "laboratory@checkupsmed.com", "lindsey.moll@checkupsmed.com", "Positive Covid Test Results " + email.Name, body, pdfStream);
                            //emailService.SendMessage("idris.maloba@checkupsmed.com", "Positive Covid Test Results " + email.Name, "PFA", pdfStream);
                        }
                        else
                        {
                            //emailService.SendMessage($"enock.bwana@checkupsmed.com","enockbwana@gmail.com", "Covid Test Results " + email.Name, "PFA", pdfStream);
                            emailService.SendMessage2($"{email.Email}", $"{email.Email2}", "idris.maloba@checkupsmed.com", "Covid Test Results " + email.Name, body, pdfStream);
                            // emailService.SendMessage(email.Email, "Test Results " + email.Name, "PFA", pdfStream);
                        }


                    }
                    else if (!string.IsNullOrWhiteSpace(email.Email) && string.IsNullOrWhiteSpace(email.Email2))
                    {
                        if (labCerts.TestValue.ToLower().Contains("positive"))
                        {
                            emailService.SendMessage2($"idris.maloba@checkupsmed.com", "laboratory@checkupsmed.com", "lindsey.moll@checkupsmed.com", "Positive Covid Test Results " + email.Name, body, pdfStream);
                            //emailService.SendMessage("idris.maloba@checkupsmed.com", "Positive Covid Test Results " + email.Name, "PFA", pdfStream);
                        }
                        else
                        {
                            //emailService.SendMessage($"enock.bwana@checkupsmed.com","enockbwana@gmail.com", "Covid Test Results " + email.Name, "PFA", pdfStream);
                            emailService.SendMessage($"{email.Email}", "idris.maloba@checkupsmed.com", "Covid Test Results " + email.Name, body, pdfStream);
                            // emailService.SendMessage(email.Email, "Test Results " + email.Name, "PFA", pdfStream);
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(email.Email) && !string.IsNullOrWhiteSpace(email.Email2))
                    {
                        if (labCerts.TestValue.ToLower().Contains("positive"))
                        {
                            emailService.SendMessage2($"idris.maloba@checkupsmed.com", "laboratory@checkupsmed.com", "lindsey.moll@checkupsmed.com", "Positive Covid Test Results " + email.Name, body, pdfStream);
                            //emailService.SendMessage("idris.maloba@checkupsmed.com", "Positive Covid Test Results " + email.Name, "PFA", pdfStream);
                        }
                        else
                        {
                            //emailService.SendMessage($"enock.bwana@checkupsmed.com","enockbwana@gmail.com", "Covid Test Results " + email.Name, "PFA", pdfStream);
                            emailService.SendMessage($"{email.Email}", "idris.maloba@checkupsmed.com", "Covid Test Results " + email.Name, body, pdfStream);
                            // emailService.SendMessage(email.Email, "Test Results " + email.Name, "PFA", pdfStream);
                        }
                    }

                }

                else
                {
                    //emailService.SendMessage($"enock.bwana@checkupsmed.com", "enockbwana@gmail.com", "Covid Test Results " + email.Name, "PFA", pdfStream);
                    emailService.SendMessage($"laboratory@checkupsmed.com", "idris.maloba@checkupsmed.com", "Covid Test Results " + email.Name, body, pdfStream);
                    //emailService.SendMessage("idris.maloba@checkupsmed.com", " Covid Test Results " + email.Name, "PFA", pdfStream);
                }


                // close pdf document
                doc.Close();
            }
            catch(Exception ex)
            {
                await lab.SendToErrorLogLab(new object[] { ex.Message });
            }
            
        }
        public async Task GenerateInvoice(string cycle_id)
        {
            try
            {
                using var ct = _serviceProvider.CreateScope();
                //order detail
                var finance = ct.ServiceProvider.GetRequiredService<IFinanceDocumentsService>();

                var receiptParams = await finance.GetInvoiceDetails(new object[] { cycle_id });

                var receiptItems = await finance.GetLabtestItemsPCR(new object[] { cycle_id });
                InvoiceParticularsVM invoicex = new InvoiceParticularsVM();
                invoicex.PatientName = receiptParams.PatientName;
                invoicex.PatientNumber = receiptParams.PatientNumber;
                invoicex.DateGenerated = receiptParams.DateGenerated;
                invoicex.InvoiceNumber = receiptParams.InvoiceNumber;
                invoicex.InvoiceItems = receiptItems;
                //pdf generator
                HtmlToPdf converter = new HtmlToPdf();
                //// set converter options
                converter.Options.PdfPageSize = PdfPageSize.A4;

                // header settings
                converter.Options.DisplayHeader = true;
                converter.Header.DisplayOnFirstPage = true;
                converter.Header.DisplayOnOddPages = true;
                converter.Header.DisplayOnEvenPages = true;
                converter.Header.Height = 128;
                //converter.Options.PdfPageOrientation = pdfOrientation;
                //converter.Options.WebPageWidth = webPageWidth;
                //converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an html string
                converter.Options.DisplayFooter = true;
                converter.Footer.DisplayOnFirstPage = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.Height = 128;



                decimal totalBill = 0;
                decimal taxTotal = 0;
                decimal savingsTotal = 0;
                string htmlString = "";

                htmlString += @$"
<!DOCTYPE html>
<html>
<head>
    <link href='C:\Invoice\Res\style.css' rel='stylesheet'>
    <link href='C:\Invoice\Res\bootsrap.min.css' rel='stylesheet' type='text/css'>
    <link href='C:\Invoice\Res\Receipt.css' rel='stylesheet' type='text/css'>    
</head>
<body>
     <div class='container'>
        <div class='panel'>
            <div class='row'>
            
            </div><br />
            <div style='margin: 20px;'>
                    <div style='color:Black' class='col-md-12 col-sm-12 panelPart'>
                        <h3><b> Invoice</b></h3>
                    </div>
                    <hr style='width:100%;border: none; border-bottom: 2px solid black;' />
            </div>
           <div style='margin: 20px;' class='row'>
                <div class='float-left'>
                    <div class='col-md-2'style='text-align: right;'>
                        <div><b>Client Name:</b></div>
                        <div><b>IP/OPD No:</b></div>
                    </div>
                    <div class='col-md-4'style='text-align: left;margin-left: -20px;'>
                        <div style='font-size:12px;'>{invoicex.PatientName}</div>
                        <div>{invoicex.PatientNumber}</div>
                    </div>
                    </div>
                    <div class='col-md-2'style='text-align: right;'>
                        <div><b>Date :</b></div>
                        <div><b>Invoice No. :</b></div>
                    </div>
                    <div class='col-md-4' style='text-align: left; margin-left: -20px;'>
                        <div>{invoicex.DateGenerated}</div>
                        <div>{invoicex.InvoiceNumber}</div>
                    </div>
                </div>

                <div style='margin: 20px;' class='row'>          
  <table class='table table-bordered'>
    <thead>
      <tr style='background-color:blue !important;'>
        <th>DATE</th>
        <th>DESCRIPTION</th>
        <th>QTY</th>
        <th>PRICE</th>
        <th>VAT</th>
        <th>TOTAL</th>
        <th>RETAIL</th>
        <th>SAVINGS</th>
      </tr>
    </thead>
    <tbody>
    ";
                foreach (var receiptItem in invoicex.InvoiceItems)
                {
                    htmlString += @$"
<tr>
        <td>{invoicex.DateGenerated}</td>
        <td>{receiptItem.Description}</td>
        <td>{receiptItem.Quantity}</td>
        <td>{receiptItem.Price}</td>
        <td>{receiptItem.VAT}</td>
        <td>{receiptItem.Total}</td>
        <td>{receiptItem.Retail}</td>
        <td>{receiptItem.Savings}</td>
      </tr>
";
                    totalBill += receiptItem.Price;
                    savingsTotal += receiptItem.Savings;
                    taxTotal += receiptItem.VAT;
                }

                htmlString += @$"</tbody>
  </table>
</div>

 <div class='row' style='margin: 20px;'>
<div class='col-md-6'>
<div class='row'>
                <div class='col-md-12 '>
                    <table>
                        <tr>
                            <td style='width:150px'><small>Customer Signature:</small></td>
                            <td class='under' style='width:450px'><small style='font-weight:bold; font-style:italic; color:black'></small></td>
                        </tr>
                    </table>
                </div>
            </div><br />
            <div class='row' style='color:black'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td style='width:150px'><small>Sign Date:&nbsp;&nbsp;&nbsp;&nbsp;</small></td>
                            <td class='under' style='width:450px'><small style='font-weight:bold; font-style:italic; color:black;'>&nbsp;&nbsp;&nbsp;</small></td>
                        </tr>
                    </table>
                </div>
            </div><br />
                  <div class='row' style='color:black'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td style='width:150px'><small>Refill Date:&nbsp;&nbsp;&nbsp;&nbsp;</small></td>
                            <td class='under' style='width:450px'><small style='font-weight:bold; font-style:italic; color:black;'>&nbsp;&nbsp;&nbsp;</small></td>
                        </tr>
                    </table>
                </div>
            </div><br />
                  <div class='row' style='color:black'>
                <div class='col-md-12'>
                    <table>
                        <tr>
                            <td style='width:150px'><small>Served By:&nbsp;&nbsp;&nbsp;&nbsp;</small></td>
                            <td class='under' style='width:450px;'><small style='font-weight:bold; font-style:italic; color:black;'>System</small></td>
                        </tr>
                    </table>
                </div>
            </div><br />
        </div>
        <div class='col-md-6'>
            <center>
            <div class='col-md-6'style='text-align: right;'>
                        <div><b>Total Bill :</b></div>
                        <div><b>Tax : </b></div>
                        <div><b>Tax : </b></div>
                    </div>
                    <div class='col-md-6' style='text-align: left; margin-left: -10px;'>
                        <div><b>Ksh {totalBill}</b></div>
                        <div><b>Ksh {taxTotal}</b></div>
                        <div><b>Ksh {taxTotal}</b></div>
                    </div>
                    </center>
                     <center>
                        <div class='col-md-2'></div>
            <div class='col-md-8'>
                     <hr style='width:100%;border: none; border-bottom: 2px solid black;' />
                 </div>
                   <div class='col-md-2'></div>
                        </center>
                              <center>
            <div class='col-md-6'style='text-align: right;'>
                        <div><b>Invoice Balance :</b></div>
                        <div><b>Savings Total : </b></div>
                        <div><b>Savings(%) : </b></div>
                    </div>
                    <div class='col-md-6' style='text-align: left; margin-left: -10px;'>
                        <div><b>Ksh {totalBill}</b></div>
                        <div><b>Ksh {savingsTotal}</b></div>
                        <div><b>{savingsTotal / totalBill * 100}%</b></div>
                    </div>
                    </center>
      </div>
        </div>
    <div style='margin: 20px;'>
        <hr style='width:100%;border: none; border-bottom: 2px solid black; margin-top: 40px;' />
          <p style='margin: 0 !important;'><i><b>Upon signing of this invoice you confirm that the drugs recieved are as per the prescription/order and are in good condition
</b></i></p>
      </div>
   <div style='margin: 20px;' class='row'>
      <div class='form-check'>
  <input
    class='form-check-input'
    type='checkbox'
    value=''
    id='flexCheckDefault'
  />
  <label for='flexCheckDefault'>
   <i><b>I consent you can contact me for refills</b></i>
  </label>
</div>
</div>
          <div style='margin: 20px;' class='row'>
          <p style='margin: 0 !important;'><i><b>Overall, how would you rate your experience with Concierge/Checkups medical services?
</b></i></p>
      </div>

<div class='row' style='margin: 20px;'>
<div class='row'>
<div class='col-md-6'>
<div class='row'>
<div class='col-md-12 '>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='flexCheckDefault'/>
  <label for='flexCheckDefault'><i><b>Very Satisfied</b></i></label>
</div>
</div>
<div class='col-md-12 '>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='flexCheckDefault'/>
  <label for='flexCheckDefault'><i><b>Satisfied</b></i></label>
</div>
</div>
<div class='col-md-12 '>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='flexCheckDefault'/>
  <label for='flexCheckDefault'><i><b>Neutral</b></i></label>
</div>
</div>
<div class='col-md-12 '>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='flexCheckDefault'/>
  <label for='flexCheckDefault'><i><b>Dissatisfied</b></i></label>
</div>
</div>
<div class='col-md-12 '>
<div class='form-check'>
  <input class='form-check-input' type='checkbox' value='' id='flexCheckDefault'/>
  <label for='flexCheckDefault'><i><b>Very dissatisfied</b></i></label>
</div>
</div>
</div>
</div>
<div class='col-md-6'>
<div class='row'>
    <center>
        <h2>Mpesa Till Number: 910502</h2>
    </center>
</div>
</div>
</div>
</div>
             <div class='row'>

</div>
         </div>
         </div>
</div>
</body>
</html>


";
                string headerImage = @"C:\Certificate\Res\header.png";
                PdfImageSection headerImg = new PdfImageSection(0, 0, 594, headerImage);
                converter.Header.Add(headerImg);

                string imgFile = @"C:\Certificate\Res\footer.png";
                PdfImageSection img = new PdfImageSection(0, 0, 594, imgFile);
                converter.Footer.Add(img);
                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                MemoryStream pdfStream = new MemoryStream();
                //string fileLocation = @"D:/SysDocs/Zidi/Certificates/invoice.pdf";
                doc.Save(pdfStream);
                pdfStream.Position = 0;

                // create email message
                EmailService emailService = new EmailService();
               // emailService.SendMessage("enock.bwana@checkupsmed.com", "Checkups Invoice", "PFA", pdfStream);

                // close pdf document
                doc.Close();
            }
            catch(Exception ex)
            {

            }
            
        }
        public async Task GenerateConsentForm(string cycle_id)
        {
            try
            {
                using var ct = _serviceProvider.CreateScope();
                //order detail
                var lab = ct.ServiceProvider.GetRequiredService<ILabDocumentsService>();
            }
            catch(Exception ex)
            {

            }
            
        }
        public void GenerateQrCode(string document_id)
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            string url = "https://registrationapi.checkupsmed.com:8002/api/v1/checkups/verification/fetch-cert?cycleId=" + document_id;
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
            Bitmap bitmap = qRCode.GetGraphic(12);
            bitmap.Save(@"C:\QR\"+ document_id + @".png");           
        }
        private string populateCovidEmailBody(string fname)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(@"C:\EmailFiles\CovidResultEmail.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{firstname}", fname);
            return body;
        }
        public async Task GenerateMemberFinancialReport(string memberno,string invoiceno)
        {
            using var ct = _serviceProvider.CreateScope();
            var ms = ct.ServiceProvider.GetRequiredService<IMemberSubscriptionService>();
            try
            {
                //pdf generator
                HtmlToPdf converter = new HtmlToPdf();
                //// set converter options
                converter.Options.PdfPageSize = PdfPageSize.A4;

                // header settings
                converter.Options.DisplayHeader = true;
                converter.Header.DisplayOnFirstPage = true;
                converter.Header.DisplayOnOddPages = true;
                converter.Header.DisplayOnEvenPages = true;
                converter.Header.Height = 128;
                converter.Options.DisplayFooter = true;
                converter.Footer.DisplayOnFirstPage = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.Height = 128;

                var CDetails = await ms.GetConsultations(new object[] {memberno,invoiceno });
                var PDetails = await ms.GetPharmacyItems(new object[] { memberno, invoiceno });
                var DDetails = await ms.GetDiagnostics(new object[] { memberno, invoiceno });
                var LDetails = await ms.GetLabs(new object[] { memberno, invoiceno });

                string htmlString = @"<!DOCTYPE html>
<html>
<head>
    <link href='C:\patientreport\Res\style.css' rel='stylesheet'>
    <link href='C:\patientreport\Res\bootsrap.min.css' rel='stylesheet' type='text/css'>
    <link href='C:\patientreport\Res\Receipt.css' rel='stylesheet' type='text/css'>    
</head>
<body>
     <div class='container'>
        <div class='panel'>
            <div class='row'>
            </div><br />
               <div style='margin: 20px;'>
                    <div style='color:Black; text-align:center' class='panelPart'>
                        <h3><b>MEMBER FINANCIAL REPORT</b></h3>
                    </div>
                    <hr style='width:100%;border: none; border-bottom: 2px solid black;' />
            </div>
 <div style='margin: 20px;'>
                <div class='row'>
                    <div class='col-md-7'>
                             <div class='float-left'>
                    <div class='col-md-5'style='text-align: left;'>
                        <div><b>PATIENT NO#</b></div>
                        <div><b>NAMES</b></div>
                        <div><b>DOB</b></div>
                        <div><b>EMAIL</b></div>
                        <div><b>PHONE</b></div>                
                    </div>
                   
                    <div class='col-md-7'style='text-align: left;'>
                        <div>: SGX56627</div>
                        <div>: Gare Wanyoike</div>
                        <div>: Male</div>
                        <div>: Kenyan</div>
                        <div>: Wamaitha Grace</div>                    
                    </div>
                      </div>
                    </div>
                    <div class='col-md-5'>
                                   <div class='float-left'>
                    <div class='col-md-4'style='text-align: left;'>
                        <div><b>NATIONALITY</b></div>
                        <div><b>ADDRESS</b></div>
						<div><b>PACKAGE</b></div>
                        <div><b>ENROLLMENT_DATE</b></div>
                        <div><b>END DATE</b></div>  
                        <div  style='margin-top:20px'><b>DATE </b></div>  						
                    </div>
                   
                    <div class='col-md-8'style='text-align: left;'>
                        <div>: 03-03-2000</div>
                        <div>: 22 Years</div>
                        <div>: 45663773</div>
                        <div>: 0705461091</div>
						<div>: 0705461091</div>
                        <div style='margin-top:20px'>: 19/05/2021</div>                        
                    </div>
                      </div>
                    </div>                     
                </div>
                  </div>
<div style='margin: 20px;' class='row'>    
        <div style='color:Black' class='panelPart'>
                        <h3><b>Subscriptions</b></h3>
                    </div>		
  <table class='table table-bordered'>
    <thead>
      <tr style='background-color:blue !important;'>
        <th rowspan='2' colspan='1'>subscriptions</th>
        <th rowspan='1'colspan='2'>Consumptions</th>
        <th rowspan='2'colspan='1'>Balance</th>
        <th rowspan='2'colspan='1'>Savings</th>
      </tr>
	  <tr>
        <th>Billable</th>
        <th>Non-Billable</th>
      </tr>
    </thead>
<tbody>
";
                //consultations
                if (CDetails != null)
                {
                    htmlString += @"
<tr colspan='5'>
        <td colspan='5'><h3><b>Consultation</b></h3></td>
      </tr>
";
                    foreach(var det in CDetails)
                    {
                        htmlString += @$"
        <tr>
        <td>{det.Item}</td>";
                        if (string.Equals(det.Charges, "Billable"))
                        {
                            htmlString += @$"<td>{det.Amount}</td>";
                            htmlString += @$"<td>0</td>";
                        }
                        else
                        {
                            htmlString += @$"<td>0</td>";
                            htmlString += @$"<td>{det.Amount}</td>";
                            
                        }

        htmlString+=@$"
        <td>23.0</td>
        <td>{det.Savings}</td>
      </tr>
";
                    }
                }

                //lab
                if (LDetails != null)
                {
                    htmlString += @"
<tr colspan='5'>
        <td colspan='5'><h3><b>Laboratory</b></h3></td>
      </tr>
";
                    foreach (var det in LDetails)
                    {
                        htmlString += @$"
        <tr>
        <td>{det.Item}</td>";
                        if (string.Equals(det.Charges, "Billable"))
                        {
                            htmlString += @$"<td>{det.Amount}</td>";
                            htmlString += @$"<td>0</td>";
                        }
                        else
                        {
                            htmlString += @$"<td>0</td>";
                            htmlString += @$"<td>{det.Amount}</td>";

                        }

                        htmlString += @$"
        <td>23.0</td>
        <td>{det.Savings}</td>
      </tr>
";
                    }
                }
                //diagnostics
                if (DDetails != null)
                {
                    htmlString += @"
<tr colspan='5'>
        <td colspan='5'><h3><b>Diagnostics</b></h3></td>
      </tr>
";
                    foreach (var det in DDetails)
                    {
                        htmlString += @$"
        <tr>
        <td>{det.Item}</td>";
                        if (string.Equals(det.Charges, "Billable"))
                        {
                            htmlString += @$"<td>{det.Amount}</td>";
                            htmlString += @$"<td>0</td>";
                        }
                        else
                        {
                            htmlString += @$"<td>0</td>";
                            htmlString += @$"<td>{det.Amount}</td>";

                        }

                        htmlString += @$"
        <td>23.0</td>
        <td>{det.Savings}</td>
      </tr>
";
                    }
                }
                //Pharmacy
                if (PDetails != null)
                {
                    htmlString += @"
<tr colspan='5'>
        <td colspan='5'><h3><b>Pharmacy</b></h3></td>
      </tr>
";
                    foreach (var det in PDetails)
                    {
                        htmlString += @$"
        <tr>
        <td>{det.Item}</td>";
                        if (string.Equals(det.Charges, "Billable"))
                        {
                            htmlString += @$"<td>{det.Amount}</td>";
                            htmlString += @$"<td>0</td>";
                        }
                        else
                        {
                            htmlString += @$"<td>0</td>";
                            htmlString += @$"<td>{det.Amount}</td>";

                        }

                        htmlString += @$"
        <td>23.0</td>
        <td>{det.Savings}</td>
      </tr>
";
                    }
                }
                //other htmsls

                htmlString += @"
</div>
             <div class='row'>
</div>
         </div>
         </div>
</div>
</body>
</html>
";
                string headerImage = @"C:\Certificate\Res\header.png";
                PdfImageSection headerImg = new PdfImageSection(0, 0, 594, headerImage);
                converter.Header.Add(headerImg);

                string imgFile = @"C:\Certificate\Res\footer.png";
                PdfImageSection img = new PdfImageSection(0, 0, 594, imgFile);
                converter.Footer.Add(img);


                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                PdfDocument doc1 = converter.ConvertHtmlString(htmlString);
                // save pdf document

                string fileLocation = @$"D:/Subscription/MemberSubscriptionReports/{invoiceno}.pdf";
                doc.Save(fileLocation);
                // create memory stream to save PDF
                MemoryStream pdfStream = new MemoryStream();
                doc1.Save(pdfStream);
                pdfStream.Position = 0;
                //fetch email
                //var email = await gen.GetEmailPhone(new object[] { cycle_id });
                //string body = populateCovidEmailBody(email.Name);

                //var gateway = new AfricasTalkingGateway(username, apikey);
                //var phoneRegex = new Regex("^[a-zA-Z0-9 +]*$");
                //string phoneto = email.Phone;
                //string message = @$"Dear {email.Name}, Thank you for choosing Checkups Medical centre. Your visit financial report is ready and sent on registered email. For any assistance please call 0111050290";
                //var sms = gateway.SendMessage("+254710226738", message, "CHECKUPS");


            }
            catch(Exception ex)
            {

            }
        }

    }
}
