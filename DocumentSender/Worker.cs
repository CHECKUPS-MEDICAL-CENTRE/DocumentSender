using DocumentSender.Models.ViewModels;
using DocumentSender.Services;
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
using System.Threading;
using System.Threading.Tasks;

namespace DocumentSender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceProvider;
        //private readonly IDocumentingService _documentingService;

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
            var unsent = await gs.GetUnsentDocuments();
            foreach (var document in unsent)
            {
                if (document.DocumentType == "invoice")
                    await GenerateInvoice(document.CycleId);
                else if (document.DocumentType == "covid_cert")
                {
                    GenerateQrCode(document.CycleId);

                    await GenerateCovidCertificate(document.CycleId);

                }
            }
           

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task GenerateCovidCertificate(string cycle_id)
        {
            using var ct = _serviceProvider.CreateScope();
            //order detail
            var lab = ct.ServiceProvider.GetRequiredService<ILabDocumentsService>();

            //get test details
            var certDetails = await lab.FetchLabCertDetails(new object[] { cycle_id});
            var testDetails = await lab.FecthLabTestParticulars(new object[] { cycle_id });
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
                    <div class='col-md-4'>
                             <div class='float-left'>
                    <div class='col-md-4'style='text-align: left;'>
                        <div><b>PAT NO#</b></div>
                        <div><b>Invoice#</b></div>
                        <div><b>Names</b></div>
                        <div><b>Gender</b></div>
                        <div><b>Age</b></div>
                        <div><b>ID/Passport</b></div>
                        <div><b>ID/Sample</b></div>
                        
                    </div>
                   
                    <div class='col-md-8'style='text-align: left;'>
                        <div>: {labCerts.PatientNumber}</div>
                        <div>: {labCerts.InvoiceNumber}</div>
                        <div>: {labCerts.PatientName}</div>
                        <div>: {labCerts.Gender}</div>
                        <div>: {labCerts.Age}</div>
                        <div>: {labCerts.IdNumber}</div>
                     <div>: {labCerts.Sample}</div>
                    </div>
                      </div>
                    </div>

                    <div class='col-md-5'>
                             <div class='float-left'>
                    <div class='col-md-5'style='text-align: left;'>
                        <div><b>Lab SN</b></div>
                        <div><b>Visit Date</b></div>
                        <div><b>Prescribed By</b></div>
                        <div><b>Date Requested</b></div>
                        <div><b>Collection Date</b></div>
                        <div><b>Report Date</b></div>
                    </div>
                   
                    <div class='col-md-7'style='text-align: left;'>
                        <div>: {labCerts.LabSerial}</div>
                        <div>: {labCerts.VisitDate}</div>
                        <div>: Dr. Grace Wamaitha</div>
                        <div>: {labCerts.VisitDate}</div>
                        <div>: {labCerts.SampleCollectionTime}</div>
                        <div>: {DateTime.UtcNow}</div>
                    </div>
                      </div>
                    </div>
                       <div class='col-md-3'>
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
                    <div class='row'>
                   <div class='col-md-12'>
                    <div style='border-color:#c6d9f0;border-style: solid;order-width: thin;border-radius:25px; color:Black' class='col-md-12 col-sm-12 panelPart'>
                        <div class='col-md-6'>
                          <h4>Result</h3>
                      </div>
                        <div class='col-md-6'>
                           <h4>Value</h3> 
                        </div>
                    </div>
                
            <div class='col-md-12' style='margin-top: 10px;'>";
            foreach(var test in testDetails)
            {
                htmlString += @$" <div class='col-md-6'>
                          <h5>{test.Test}</h5>
                      </div>

                        <div class='col-md-6'>
                           <h5>{test.Value}</h5> 
                        </div> ";
            }
                         


                  htmlString+=@$" <div style='margin-top: 20px;'>
                   <p style='margin: 0 !important;'><FONT FACE='Trebuchet MS, serif'><b>Review Notes</b></FONT></p>
                      <ul>
                        <li style='margin-bottom: 10px;'>This sample tested {labCerts.TestValue} for SARS-COV-2 virus Antigen</li>
                        <li style='margin-bottom: 10px;'>Checkups Medical Centre Molecular Lab uses SARS-CoV-2 rapid antigen test device that is  highly sensitive and specific to SARS-CoV-2 virus.</li>
                        <li style='margin-bottom: 10px;'>This is a screening test, a negative result does not preclude SARS-CoV-2 infection. A repeat test/RT-PCR is recommended if epidemiologically or clinically indicated.
                        </li>
                      </ul>
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
  <div style='position: absolute;top: 50%;left: 50%;transform: translate(-50%, -50%);'>19-07-2021</div>
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
            // save pdf document
            // create memory stream to save PDF
            MemoryStream pdfStream = new MemoryStream();
            //string fileLocation = @"D:/SysDocs/Zidi/Certificates/invoice.pdf";
            if (labCerts.TestValue.ToLower() == "positive")
            {
                doc.Save($"D:/SysDocs/Zidi/Vouchers/" + cycle_id + ".pdf");
            }
            else
            {
                doc.Save(pdfStream);
                pdfStream.Position = 0;

                // create email message
                EmailService emailService = new EmailService();
                emailService.SendMessage("enock.bwana@checkupsmed.com", "Test Results", "PFA", pdfStream);
            }
                      // close pdf document
            doc.Close();
        }
        public async Task GenerateInvoice(string cycle_id)
        {
            using var ct = _serviceProvider.CreateScope();
            //order detail
            var finance = ct.ServiceProvider.GetRequiredService<IFinanceDocumentsService>();

            var receiptParams = await finance.GetInvoiceDetails(new object[] { cycle_id });

            var receiptItems = await finance.GetLabtestItemsPCR(new object[] { cycle_id});
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
                        <div>{invoicex.PatientName}</div>
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
            emailService.SendMessage("enock.bwana@checkupsmed.com", "Test Results", "PFA", pdfStream);

            // close pdf document
            doc.Close();
        }
        public void GenerateQrCode(string document_id)
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            string url = "labverification.checkupsmed.com/kenya?id=" + document_id;
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
            Bitmap bitmap = qRCode.GetGraphic(12);
            bitmap.Save(@"C:\QR\"+ document_id + @".png");           
        }

    }
}
