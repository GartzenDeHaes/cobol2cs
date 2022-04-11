//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using DOR.WorkingStorage;
//using System.ComponentModel;
//using System.Collections.Concurrent;
//using DOR.Core;
//using DOR.Core.Data;
//using System.Threading;
//using DOR.Core.Logging;
//using DOR.Core.Data.Tandem;
//using System.Data;
//using DOR.WINOTIS;

//namespace DOR.WPF
//{
//    public class CobolViewModelBase : ViewModelBaseWinOTIS
//    {
//        public int MenuScreenNumber
//        {
//            get;
//            private set;
//        }

//        // if you use BackgroundWorker, this event can be used to notify the code behind when loading is complete.
//        public DelegateNoArgs LoadComplete;

//        public IBufferOffset ClDestPgmfIdRec
//        {
//            get;
//            private set;
//        }

//        public IBufferOffset ClAdvisoryMessageRec
//        {
//            get;
//            private set;
//        }

//        public IBufferOffset WsExitRequesterSwtRec
//        {
//            get;
//            private set;
//        }

//        public IBufferOffset ClAdvisoryIndRec
//        {
//            get;
//            private set;
//        }

//        // This is used to convert async windows key strokes into synchronous ACCEPT calls
//        public BlockingCollection<string> _keyQueue = new BlockingCollection<string>();
//        public BlockingCollection<string> KeyQueue
//        {
//            get { return _keyQueue; }
//        }

//        protected ISqlDataAccess Connection
//        {
//            get;
//            set;
//        }

//        protected Thread _myThread;

//        protected List<DelegateNoArgs> _turnTemps = new List<DelegateNoArgs>();

//        protected ILogger Logger
//        {
//            get;
//            set;
//        }

//        public CobolViewModelBase(int menuScreenNumber)
//        {
//            MenuScreenNumber = menuScreenNumber;

//            Connection = new TandemDirectSql();
//            Connection.ConnectionString = "servlet-et";
//        }

//        public override void Initialize(IContext ctx, IScreen screen, IScreenConfig cfg)
//        {
//            base.Initialize(ctx, screen, cfg);

//            Logger = ctx.Log;

//            if (!HasAccessToScreen(screen.ScreenNumber))
//            {
//                throw new AccessControlException(screen.ScreenNumber);
//            }

//            _myThread = new Thread(new ThreadStart(Main));
//            _myThread.IsBackground = true;
//            _myThread.Start();

//            string msg = ClAdvisoryMessageRec.ToString();
//            if (!String.IsNullOrWhiteSpace(msg))
//            {
//                Context.StatusBarMessage = msg;
//                ClAdvisoryMessageRec.RaisePropertyChanged();
//            }
//            else
//            {
//                Context.StatusBarMessage = "Ready";
//            }
//        }

//        protected virtual void Main()
//        {
//            throw new NotImplementedException();
//        }

//        protected void Init
//        (
//            IBufferOffset clDestPgmfId,
//            IBufferOffset clAdvisoryMessage,
//            IBufferOffset wsExitRequesterSwt,
//            IBufferOffset clAdvisoryInd
//        )
//        {
//            ClDestPgmfIdRec = clDestPgmfId;
//            ClAdvisoryMessageRec = clAdvisoryMessage;
//            WsExitRequesterSwtRec = wsExitRequesterSwt;
//            ClAdvisoryIndRec = clAdvisoryMessage;
//        }

//        public bool HasAccessToScreen(int screenNum)
//        {
//            string sql =
//                " SELECT" +
//                "    1" +
//                " FROM" +
//                "    [=defineOf]ONEREC" +
//                " WHERE" +
//                "    EXISTS ( SELECT" +
//                "             1" +
//                "          FROM" +
//                "             [=defineOf]PGMF PGMF, [=defineOf]USERCAP USERCAP" +
//                "          WHERE" +
//                "              PGMF.DORCAP_ID = USERCAP.DORCAP_ID AND" +
//                "              PGMF_ID = " + screenNum + " AND" +
//                "              DORUSER_ID = " + Context.User.DorUserId +
//                "          FOR BROWSE ACCESS" +
//                "    )" +
//                " FOR BROWSE ACCESS";

//            IDataReader reader = Connection.ExecuteReader(sql, DateTime.Now.AddHours(1));
//            return reader.RecordsAffected != 0;
//        }

//        public bool GotoScreen(int newScreenNum, int oldScreenNum, string args)
//        {
//            if (!HasAccessToScreen(newScreenNum))
//            {
//                ClAdvisoryMessageRec.Set("You do not have access to screen " + newScreenNum);
//                if (Context != null)
//                {
//                    Context.StatusBarMessage = ClAdvisoryMessageRec.ToString();
//                }
//                return false;
//            }

//            IScreen screen = Context.ScreenManager.FindOpenScreen(oldScreenNum)[0];
//            Context.ScreenManager.UpdateScreen(screen, newScreenNum, args);

//            return true;
//        }

//        protected bool ExecuteCommandLine(IBufferOffset clText, int screenNumber)
//        {
//            string commandLine = clText.ToString();
//            if (String.IsNullOrWhiteSpace(commandLine))
//            {
//                return false;
//            }

//            int pos = commandLine.IndexOf(' ');
//            int commandLineId = pos > -1 ? Int32.Parse(commandLine.Substring(0, pos)) : Int32.Parse(commandLine);

//            return GotoScreen
//            (
//                commandLineId,
//                screenNumber,
//                commandLine.Substring(commandLineId.ToString().Length)
//            );
//        }

//        protected void _9700cScreenPrint()
//        {
//            ClAdvisoryMessageRec.Set("F8-Print disabled; Press Shift-PrtScn to copy the screen to the clipboard");
//            ClAdvisoryIndRec.Set(1);
//        }

//        protected void _9710cTimeout()
//        {
//        }

//        protected void _9720cTimeoutCheck()
//        {
//        }

//        protected void _9730cInvalidKey()
//        {
//            ClAdvisoryMessageRec.Set("Invalid function key pressed");
//            ClAdvisoryIndRec.Set(1);
//        }

//        protected void _9740cApplicationHelp()
//        {
//        }

//        protected void _9800cPrevFunction()
//        {
//        }

//        protected void _9810cMenuGotoFunction()
//        {
//            ClDestPgmfIdRec.Set(MenuScreenNumber);
//            WsExitRequesterSwtRec.Set("Y");
//        }

//        protected void _9820cSwapNextFunction()
//        {
//            ClAdvisoryMessageRec.Set("Invalid function key pressed");
//            ClAdvisoryIndRec.Set(1);
//        }

//        protected void _9830cSystemMenu()
//        {
//            ClDestPgmfIdRec.Set(99);
//            WsExitRequesterSwtRec.Set("Y");
//        }

//        #region IDataErrorInfo Members

//        public override string Error
//        {
//            get
//            {
//                return ClAdvisoryMessageRec.ToString().Trim();
//            }
//        }

//        public override string this[string columnName]
//        {
//            get
//            {
//                //if (columnName == "TraSearch")
//                //{
//                //	return ValidationHelper.Length("Search text", TraSearch, 4, 60);
//                //}
//                return String.Empty;
//            }
//        }

//        #endregion

//        public override void Dispose()
//        {
//            KeyQueue.Dispose();
//            base.Dispose(true);
//        }

//        protected void ExitRequester(IBufferOffset ClDestPgmfId, IBufferOffset ClText)
//        {
//            int id = Config.CommandLine.ID == 0 ? Config.ScreenCode : Config.CommandLine.ID;

//            if (ClDestPgmfId != 0)
//            {
//                if (!String.IsNullOrWhiteSpace(ClText.ToString()))
//                {
//                    if (ExecuteCommandLine(ClText, id))
//                    {
//                        return;
//                    }
//                }
//                else
//                {
//                    if (GotoScreen(ClDestPgmfId.ToInt32(), id, ""))
//                    {
//                        return;
//                    }
//                }
//            }

//            GotoScreen(MenuScreenNumber, id, "");
//        }
//    }
//}
