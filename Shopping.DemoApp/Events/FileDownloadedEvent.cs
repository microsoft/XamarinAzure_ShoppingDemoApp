using System;
using Microsoft.WindowsAzure.MobileServices.Eventing;

namespace Shopping.DemoApp.Events
{
    public class FileDownloadedEvent : IMobileServiceEvent
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name understood as the item id.</value>
        public string Name
        {
            get; set;
        }
    }
}

