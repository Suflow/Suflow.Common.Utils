//using System;
//using System.Linq;
//using System.Web.UI.HtmlControls;
//using System.Web.UI.WebControls;
//using System;

//namespace Suflow.Common.Utils
//{
//    public class WebUnitTestHelper : HtmlTable
//    {
//        #region [Private]

//        private readonly object _testMethodsHolder;

//        private HtmlTable _innerTable;

//        private TextBox _messageTextBox;

//        private HtmlGenericControl _htmlMessageHolder; 

//        private HtmlTable CreateInnerTable()
//        {
//            var table = new HtmlTable();

//            //Header
//            var header = new HtmlTableRow();
//            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Nome de teste" });
//            header.Cells.Add(new HtmlTableCell("th") { InnerText = "" });
//            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Resultado" });
//            table.Rows.Add(header);

//            //Body
//            foreach (var method in from method in _testMethodsHolder.GetType().GetMethods()
//                                   from attribute in method.GetCustomAttributes(true).OfType<UnitTestAttribute>()
//                                   select method)
//                table.Rows.Add(CreateTestRow(method.Name));

//            //style
//            table.Attributes.Add("class", "Grid");
//            return table;
//        }

//        private HtmlTableRow CreateTestRow(string testName)
//        {
//            var testRow = new HtmlTableRow();

//            //Test Name
//            var lblNameCell = new HtmlTableCell { InnerText = testName.Replace('_', ' ') };
//            lblNameCell.Style.Add("width", "200px");

//            var highlight = from m in _testMethodsHolder.GetType().GetMethods()
//                    where m.Name.Equals(testName)
//                    from a in m.GetCustomAttributes(true).OfType<HighlightAttribute>()
//                    select m;
//            if(highlight.Count() > 0)
//                lblNameCell.Style.Add("color", "red");
//            testRow.Cells.Add(lblNameCell);

//            //Run Button
//            var btnRun = new Button
//            {
//                ID = "btn" + testName,
//                CommandArgument = testName,
//                CommandName = testName,
//                Text = @"Run"
//            };
//            btnRun.Click += BtnRunClick;
//            var btnRunCell = new HtmlTableCell();
//            btnRunCell.Style.Add("width", "100px");
//            btnRunCell.Controls.Add(btnRun);
//            testRow.Cells.Add(btnRunCell);

//            //Result Label
//            var lblResult = new Label
//            {
//                ID = "lbl" + testName
//            };
//            var lblResultCell = new HtmlTableCell();
//            lblResultCell.Style.Add("width", "100px");
//            lblResultCell.Controls.Add(lblResult);
//            testRow.Cells.Add(lblResultCell);

//            return testRow;
//        }

//        private void BtnRunClick(object sender, EventArgs e)
//        {
//            var btn = (Button)sender;
//            var tableRow = ((HtmlTableRow)btn.Parent.Parent);
//            var lblResult = (Label)tableRow.Cells[2].Controls[0];

//            try
//            {
//                var methodInfo = _testMethodsHolder.GetType().GetMethod(btn.CommandName);
//                var returnedObject = methodInfo.Invoke(_testMethodsHolder, new object[] { });
//                lblResult.Text = @"Success";
//                lblResult.Style.Add("color", "green");

//                _messageTextBox.Visible = returnedObject != null;
//                if (returnedObject != null)
//                    _messageTextBox.Text = returnedObject.ToString();

//                _htmlMessageHolder.Visible = !string.IsNullOrEmpty(HtmlMessage);
//                if (HtmlMessage != null)
//                    _htmlMessageHolder.InnerHtml = HtmlMessage;
//            }
//            catch (Exception exp)
//            {
//                lblResult.Text = @"Failed";
//                lblResult.Style.Add("color", "red");
//                _messageTextBox.Visible = true;
//                _messageTextBox.Text = exp.InnerException.MessageWithStackTrace();
//            }
//            if (!string.IsNullOrEmpty(_messageTextBox.Text))
//            {
//                _messageTextBox.Text = @"Teste: " + btn.CommandName.Replace('_', ' ')
//                                       + Environment.NewLine + @"Data: " + DateTime.Now
//                                       + Environment.NewLine + @"Retorno: " + _messageTextBox.Text;
//            }
//            _messageTextBox.Height = Unit.Pixel(26 * _innerTable.Rows.Count);
//            _htmlMessageHolder.Style.Add("Height", _messageTextBox.Height.Value + "px");
//        }

//        #endregion

//        public string HtmlMessage { get; set; }

//        public WebUnitTestHelper(object testMethodsHolder)
//        {
//            _testMethodsHolder = testMethodsHolder;
//        } 

//        protected override void OnLoad(EventArgs e)
//        {
//            base.OnLoad(e);

//            //Row
//            var row = new HtmlTableRow();
//            Rows.Add(row);

//            //InnerTableCell
//            _innerTable = CreateInnerTable();
//            var innnerTableCell = new HtmlTableCell() { Width = "425px" };
//            innnerTableCell.Controls.Add(_innerTable);
//            row.Cells.Add(innnerTableCell);


//            //MessageCell 
//            _messageTextBox = new TextBox
//                                  {
//                                      ID = "messageTextBox",
//                                      Visible = false,
//                                      Width = Unit.Percentage(100),
//                                      ReadOnly = true,
//                                      TextMode = TextBoxMode.MultiLine,
//                                      EnableViewState = false,
//                                      Wrap = false
//                                  };
//            var messageTableCell = new HtmlTableCell { Width = "425px" };
//            messageTableCell.Controls.Add(_messageTextBox);
//            row.Cells.Add(messageTableCell);

//            //HtmlMessageCell
//            _htmlMessageHolder = new HtmlGenericControl("div")
//            {
//                ID = "htmlMessageHolder",
//                Visible = false, 
//                EnableViewState = false
//            };
//            _htmlMessageHolder.Style.Add("overflow", "auto");
//            var htmlMessageTableCell = new HtmlTableCell { Width = "225px" };
//            htmlMessageTableCell.Controls.Add(_htmlMessageHolder);
//            row.Cells.Add(htmlMessageTableCell);
//        }

//        public class UnitTestAttribute : Attribute
//        {
//        }

//        public class HighlightAttribute: Attribute
//        {
//        }
//    }
//}
