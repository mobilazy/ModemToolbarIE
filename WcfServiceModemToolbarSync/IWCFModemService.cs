using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ModemWebUtility;
using System.Windows.Forms;
using System.IO;
using ModemWebUtility;

namespace WcfServiceModemToolbarSync
{



    // Define a duplex service contract.
    // A duplex contract consists of two interfaces.
    // The primary interface is used to send messages from client to service.
    // The callback interface is used to send messages from service back to client.
    // ICalculatorDuplex allows one to perform multiple operations on a running result.
    // The result is sent back after each operation on the ICalculatorCallback interface. \\Namespace = "http://Microsoft.ServiceModel.Samples", SessionMode = SessionMode.Required, 
    [ServiceContract] //(Namespace = "http://Microsoft.ServiceModel.Samples", SessionMode = SessionMode.Required, CallbackContract = typeof(IWCFModemServiceCallback))]
    public interface IWCFModemService
    {


        [OperationContract]
        int GetSyncStatus();


        //[OperationContract(IsOneWay = true)]
        //void CallbackMenuListItemClass();

        //[OperationContract(IsOneWay = true)]
        //void CallbackSearchBoxItemClass();

        //[OperationContract(IsOneWay = true)]
        //void CallbackLinkListItemClass();

        [OperationContract]
        MenuListClass GetMenuListItemClasses();

        [OperationContract]
        SearchListClass GetSearchBoxItemClasses();

        [OperationContract]
        LinkListClass GetLinkListItemClasses();

        
        // TODO: Add your service operations here
    }

    //public interface IWCFModemServiceCallback
    //{
    //    [OperationContract(IsOneWay = true)]
    //    void Alive();
    //    [OperationContract(IsOneWay = true)]
    //    void SyncStatusEquals(int status);
    //    [OperationContract(IsOneWay = true)]
    //    void DataMenuListItemClassEquals(MenuListClass menuListItems);
    //    [OperationContract(IsOneWay = true)]
    //    void DataSearchBoxItemClassEquals(SearchListClass searchBoxItems);
    //    [OperationContract(IsOneWay = true)]
    //    void DataLinkListItemClassEquals(LinkListClass linkListItems);

    //}

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfServiceModemToolbarSync.ContractType".

    [DataContract]
    public class SearchListClass
    {
        [DataMember]
        public List<SearchBoxItemClass> List { get; set; } = new List<SearchBoxItemClass>();
    }

    [DataContract]
    public class LinkListClass
    {
        [DataMember]
        public List<LinkListItemClass> List { get; set; } = new List<LinkListItemClass>();
    }

    [DataContract]
    public class MenuListClass
    {
        [DataMember]
        public List<MenuListItemClass> List { get; set; } = new List<MenuListItemClass>();
    }

    [DataContract]
    public class MenuListItemClass
    {
        private string caption;
        private string hint;
        private KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>[] links;
        private byte[] img;

        [DataMember]
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        [DataMember]
        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        [DataMember]
        public KeyValuePair<string, KeyValuePair<string, ModemMwdPostObjects>[]>[] Links
        {
            get { return links; }
            set { links = value; }
        }

        [DataMember]
        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }

    }

    

    [DataContract]
    public class SearchBoxItemClass
    {
        private string clearHistoryText;
        private string greetingText;
        private string searchURL;
        private string searchBoxTooltip;
        private string buttonText;
        private string buttonTooltip;
        private Size inputBoxSize;
        private FlatStyle inputBoxFlatStyle;
        private byte[] img;


        [DataMember]
        public string ClearHistoryText
        {
            get { return clearHistoryText; }
            set { clearHistoryText = value; }
        }

        [DataMember]
        public string GreetingText
        {
            get { return greetingText; }
            set { greetingText = value; }
        }

        [DataMember]
        public string SearchURL
        {
            get { return searchURL; }
            set { searchURL = value; }
        }

        [DataMember]
        public string SearchBoxTooltip
        {
            get { return searchBoxTooltip; }
            set { searchBoxTooltip = value; }
        }

        [DataMember]
        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; }
        }

        [DataMember]
        public string ButtonTooltip
        {
            get { return buttonTooltip; }
            set { buttonTooltip = value; }
        }

        [DataMember]
        public Size InputBoxSize
        {
            get { return inputBoxSize; }
            set { inputBoxSize = value; }
        }

        [DataMember]
        public FlatStyle InputBoxFlatStyle
        {
            get { return inputBoxFlatStyle; }
            set { inputBoxFlatStyle = value; }
        }

        [DataMember]
        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }


    }

    [DataContract]
    public class LinkListItemClass
    {
        private string caption;
        private string hint;
        private KeyValuePair<string, string>[] links;
        private byte[] img;

        [DataMember]
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        [DataMember]
        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        [DataMember]
        public KeyValuePair<string, string>[] Links
        {
            get { return links; }
            set { links = value; }
        }

        [DataMember]
        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }

    }

}
