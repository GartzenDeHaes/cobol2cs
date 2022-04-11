//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using DOR.WPF;
//using DOR.WINOTIS;
//using System.ComponentModel;
//using System.Windows.Input;
//using System.Windows;
//using System.Threading;
//using GalaSoft.MvvmLight.Threading;
//using System.Windows.Controls;
//using System.Windows.Data;

//namespace DOR.WorkingStorage.Wpf
//{
//    public class CobolScreenBase : DOR.WINOTIS.ScreenBase
//    {
//        public CobolScreenBase()
//        {
//        }

//        public override void Initialize(IContext ctx, IScreenConfig cfg)
//        {
//            base.Initialize(ctx, cfg);

//            ((CobolViewModelBase)ViewModel).LoadComplete += () =>
//            {
//                ViewModel.Context.WaitIndicatorVisibility = Visibility.Hidden;
//            };

//            ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//            FocusFirstControl();
//        }

//        public void FocusFirstControl()
//        {
//            BackgroundWorker worker = new BackgroundWorker();
//            worker.DoWork += (ss, ee) =>
//            {
//                Thread.Sleep(500);
//                DispatcherHelper.CheckInvokeOnUI(() =>
//                {
//                    FocusManager.SetFocusedElement(this, this);
//                    UIElement element = FocusManager.GetFocusedElement(this) as UIElement;
//                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
//                });
//            };
//            worker.RunWorkerAsync();
//        }

//        protected virtual void GrdScreen_KeyUp(Key e)
//        {
//            if (Keyboard.FocusedElement is TextBox)
//            {
//                switch (e)
//                {
//                    case Key.F1:
//                    case Key.F2:
//                    case Key.F3:
//                    case Key.F4:
//                    case Key.F5:
//                    case Key.F6:
//                    case Key.F7:
//                    case Key.F8:
//                    case Key.F9:
//                    case Key.F10:
//                    case Key.F11:
//                    case Key.F12:
//                        BindingOperations.GetBindingExpression((TextBox)Keyboard.FocusedElement, TextBox.TextProperty).UpdateSource();
//                        break;
//                }
//            }

//            switch (e)
//            {
//                case Key.F1:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF1-KEY" : "F1-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F2:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF2-KEY" : "F2-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F3:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF3-KEY" : "F3-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F4:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF4-KEY" : "F4-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F5:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF5-KEY" : "F5-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F6:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF6-KEY" : "F6-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F7:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF7-KEY" : "F7-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F8:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF8-KEY" : "F8-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F9:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF9-KEY" : "F9-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F10:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF10-KEY" : "F10-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F11:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF11-KEY" : "F11-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//                case Key.F12:
//                    ((CobolViewModelBase)ViewModel).KeyQueue.Add((Keyboard.Modifiers == ModifierKeys.Shift ? "SF12-KEY" : "F12-KEY"));
//                    ViewModel.Context.WaitIndicatorVisibility = Visibility.Visible;
//                    break;
//            }
//        }

//        protected virtual void TextBox_GotFocus(object sender, RoutedEventArgs e)
//        {
//            ((TextBox)sender).SelectAll();
//        }

//        protected virtual void AutoTab_KeyUp(object sender, KeyEventArgs e)
//        {
//            if (((TextBox)sender).MaxLength == ((TextBox)sender).Text.Length)
//            {
//                if (((TextBox)sender).CaretIndex == ((TextBox)sender).Text.Length)
//                {
//                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
//                    ((UIElement)sender).MoveFocus(request);
//                }
//            }
//        }

//        protected virtual void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
//        {
//            UIElement element = sender as UIElement;

//            switch (e.Key)
//            {
//                case Key.Home:
//                case Key.Up:
//                    if (element != null)
//                    {
//                        element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
//                    }
//                    break;
//                case Key.End:
//                case Key.Down:
//                    if (element != null)
//                    {
//                        element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
//                    }
//                    break;
//                case Key.Left:
//                    {
//                        if (element != null)
//                        {
//                            if (!(element is TextBox))
//                            {
//                                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
//                                e.Handled = true;
//                                break;
//                            }
//                            if (((TextBox)element).CaretIndex == ((TextBox)element).Text.Length)
//                            {
//                                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
//                                e.Handled = true;
//                                break;
//                            }
//                        }
//                        break;
//                    }
//                case Key.Right:
//                    {
//                        if (element != null)
//                        {
//                            if (!(element is TextBox))
//                            {
//                                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
//                                e.Handled = true;
//                                break;
//                            }
//                            if (((TextBox)element).CaretIndex == ((TextBox)element).Text.Length)
//                            {
//                                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
//                                e.Handled = true;
//                                break;
//                            }
//                        }
//                        break;
//                    }
//            }
//        }
//    }
//}
