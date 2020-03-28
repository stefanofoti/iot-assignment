using System;

namespace IotWork.Models
{
    public class DataItem
    {
        private string _id { get; set; }
        private string _name { get; set; }
        private string _value { get; set; }
        private float _temperature { get; set; }
        private float _humidity { get; set; }
        private float _windDirection { get; set; }
        private float _windIntensity { get; set; }
        private float _rain { get; set; }


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

        public void setTemperature(float s)
        {
            _temperature=s;
        }


        public float getTemperature(){
            return _temperature;
        }

        public void setHumidity(float s)
        {
            _humidity=s;
        }


        public float getHumidity(){
            return _humidity;
        }

        public void setWindDirection(float s)
        {
            _windDirection=s;
        }


        public float getWindDirection(){
            return _windDirection;
        }

        public void setWindIntensity(float s)
        {
            _windIntensity=s;
        }


        public float getWindIntensity(){
            return _windIntensity;
        }

        public void setRain(float s)
        {
            _rain=s;
        }


        public float getRain(){
            return _rain;
        }
    }
}