// <copyright file="ShellViewModel.cs" company="VacuumBreather">
//      Copyright © 2014 VacuumBreather. All rights reserved.
// </copyright>
// <license type="X11/MIT">
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
//      The above copyright notice and this permission notice shall be included in
//      all copies or substantial portions of the Software.
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//      THE SOFTWARE.
// </license>

namespace TrainTool.ViewModel
{
    #region Using Directives

    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Dialogs;
    using Helpers;
    using Model;
    using Properties;
    using Services;

    #endregion

    /// <summary>
    ///     The ViewModel of the main shell.
    /// </summary>
    [Export]
    public sealed class ShellViewModel : Conductor<IScreen>
    {
        #region Constants

        private const string DefaultName = "Untitled";

        #endregion

        #region Readonly & Static Fields

        private readonly TrainSetViewModel _trainSetViewModel;
        private readonly BusyWatcher _busyWatcher;
        private readonly IApplicationService _applicationService;
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        private readonly IWindowManager _windowManager;

        #endregion

        #region Fields

        private string _currentFileName;
        private bool _isBusy;
        private bool _isModelDirty;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShellViewModel" /> class.
        /// </summary>
        /// <param name="trainSetViewModel">The train view model.</param>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="applicationService">The application service.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="busyWatcher">The busy watcher.</param>
        [ImportingConstructor]
        public ShellViewModel(TrainSetViewModel trainSetViewModel,
                              BusyWatcher busyWatcher,
                              IApplicationService applicationService,
                              IFileService fileService,
                              IDialogService dialogService,
                              IWindowManager windowManager)
        {
            Contract.Requires<ArgumentNullException>(busyWatcher != null);

            this._trainSetViewModel = trainSetViewModel;

            this._busyWatcher = busyWatcher;
            this._fileService = fileService;
            this._applicationService = applicationService;
            this._dialogService = dialogService;
            this._windowManager = windowManager;

            this._busyWatcher.BusyChanged += (sender, e) => { IsBusy = this._busyWatcher.IsBusy; };

            this._trainSetViewModel.TrainSet = new TrainSet();

            ActivateItem(this._trainSetViewModel);
            this._trainSetViewModel.ModelChanged += (sender, e) => IsModelDirty = true;

            UpdateDisplayName();
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///     Gets a value indicating whether a new train set can be created.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a new train set can be created; otherwise, <c>false</c>.
        /// </value>
        public bool CanCreateNewTrainSetAsync
        {
            get
            {
                return !IsBusy;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether a train set can be opened.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a train set can be opened; otherwise, <c>false</c>.
        /// </value>
        public bool CanOpenTrainSetAsync
        {
            get
            {
                return !IsBusy;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the current train set can be saved.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the current train set can be saved; otherwise, <c>false</c>.
        /// </value>
        public bool CanSaveTrainSetAsAsync
        {
            get
            {
                return !IsBusy;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the current train set can be saved under the last filename.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the current train set can be saved under the last filename; otherwise, <c>false</c>.
        /// </value>
        public bool CanSaveTrainSetAsync
        {
            get
            {
                return !IsBusy && !String.IsNullOrEmpty(CurrentFileName) && IsModelDirty;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the AboutBox can be shown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the AboutBox can be shown; otherwise, <c>false</c>.
        /// </value>
        public bool CanShowAboutBox
        {
            get
            {
                return !IsBusy;
            }
        }

        /// <summary>
        ///     Gets the name of the current file.
        /// </summary>
        /// <value>
        ///     The name of the current file.
        /// </value>
        public string CurrentFileName
        {
            get
            {
                return this._currentFileName;
            }
            private set
            {
                this._currentFileName = value;

                UpdateDisplayName();
                Refresh();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the application is busy with an async operation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the application is busy with an async operation; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            private set
            {
                this._isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
                Refresh();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the model has any unsaved changes.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the model has any unsaved changes; otherwise, <c>false</c>.
        /// </value>
        public bool IsModelDirty
        {
            get
            {
                return this._isModelDirty;
            }
            set
            {
                this._isModelDirty = value;
                UpdateDisplayName();
                Refresh();
            }
        }

        #endregion

        #region Instance Methods

        /// <summary>
        ///     Determines whether the current train set can be closed.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public override async void CanClose(Action<bool> callback)
        {
            await CloseTrainSetAsync(callback);
        }

        /// <summary>
        ///     Creates a new train set.
        /// </summary>
        public async Task CreateNewTrainSetAsync()
        {
            bool isCurrentTrainSetClosable = false;

            await CloseTrainSetAsync(canClose => isCurrentTrainSetClosable = canClose);

            if (!isCurrentTrainSetClosable)
            {
                return;
            }

            this._trainSetViewModel.TrainSet = new TrainSet();

            CurrentFileName = null;
            IsModelDirty = false;
        }

        /// <summary>
        ///     Shuts the application down.
        /// </summary>
        public void Exit()
        {
            CanClose(
                canClose =>
                {
                    if (canClose)
                    {
                        this._applicationService.Shutdown();
                    }
                });
        }

        /// <summary>
        ///     Opens a train set.
        /// </summary>
        public async void OpenTrainSetAsync()
        {
            bool isCurrentTrainSetClosable = false;

            await CloseTrainSetAsync(canClose => isCurrentTrainSetClosable = canClose);

            if (!isCurrentTrainSetClosable)
            {
                return;
            }

            using (Stream stream = this._fileService.Open())
            {
                if (stream == null)
                {
                    return;
                }

                try
                {
                    using (this._busyWatcher.GetTicket())
                    {
                        TrainSet trainSet = await TrainSetSerializer.LoadFromAsync(stream);
                        this._trainSetViewModel.TrainSet = trainSet;
                        CurrentFileName = this._fileService.LastFile;
                        IsModelDirty = false;
                    }
                }
                catch (Exception exception)
                {
                    ShowErrorDialog(exception, Resources.UnableToOpen);
                }
            }
        }

        /// <summary>
        ///     Saves the train set in a user specified file.
        /// </summary>
        public async Task SaveTrainSetAsAsync()
        {
            using (Stream stream = this._fileService.SaveAs())
            {
                if (stream == null)
                {
                    return;
                }

                try
                {
                    using (this._busyWatcher.GetTicket())
                    {
                        await TrainSetSerializer.SaveToAsync(this._trainSetViewModel.TrainSet, stream);
                        CurrentFileName = this._fileService.LastFile;
                        IsModelDirty = false;
                    }
                }
                catch (Exception exception)
                {
                    ShowErrorDialog(exception, Resources.UnableToSave);
                }
            }
        }

        /// <summary>
        ///     Saves the train set in the last specified file.
        /// </summary>
        public async Task SaveTrainSetAsync()
        {
            using (Stream stream = this._fileService.Save())
            {
                if (stream == null)
                {
                    ShowErrorDialog(new IOException(), Resources.UnableToSave);
                    return;
                }

                try
                {
                    using (this._busyWatcher.GetTicket())
                    {
                        await TrainSetSerializer.SaveToAsync(this._trainSetViewModel.TrainSet, stream);
                        IsModelDirty = false;
                    }
                }
                catch (Exception exception)
                {
                    ShowErrorDialog(exception, Resources.UnableToSave);
                }
            }
        }

        /// <summary>
        ///     Shows the about box.
        /// </summary>
        public void ShowAboutBox()
        {
            this._windowManager.ShowDialog(new AboutViewModel());
        }

        private async Task CloseTrainSetAsync(Action<bool> canCloseCallback)
        {
            if (!IsModelDirty)
            {
                canCloseCallback(true);
                return;
            }

            DialogResult result =
                this._dialogService.ShowDialog(
                    string.Format(
                        Resources.SaveChangesTo,
                        String.IsNullOrEmpty(CurrentFileName) ? DefaultName : new FileInfo(CurrentFileName).FullName),
                    DialogButtons.YesNoCancel);

            if (result == DialogResult.Cancel)
            {
                canCloseCallback(false);
            }
            else if (result == DialogResult.No)
            {
                canCloseCallback(true);
            }
            else
            {
                if (CanSaveTrainSetAsync)
                {
                    await SaveTrainSetAsync();
                }
                else
                {
                    await SaveTrainSetAsAsync();
                }

                canCloseCallback(!IsModelDirty);
            }
        }

        private void ShowErrorDialog(Exception exception, string message)
        {
            this._dialogService.ShowError(exception, message);
        }

        private void UpdateDisplayName()
        {
            string fileName = String.IsNullOrEmpty(CurrentFileName) ? DefaultName : new FileInfo(CurrentFileName).Name;
            string dirtyMarker = IsModelDirty ? "*" : string.Empty;

            DisplayName = String.Format("{0}{1} - {2}", fileName, dirtyMarker, ApplicationInfo.ProductTitle);
        }

        #endregion
    }
}