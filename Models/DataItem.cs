using System;

namespace IotWork.Models
{
    public class DataItem
    {
        public string _id { get; set; }
        public string _name { get; set; }
        public string _value { get; set; }

        public void setMessageId(string id)
        {
            _id=id;
        }

        public string getMessageId(){
            return _id;
        }

        public void setValue(string message)
        {
            _value=message;
        }


        public string getValue(){
            return _value;
        }
    }
}