using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MonsterTradingCards.Webserver
{
    /// <summary>This class represents a HTTP path param.</summary>
    public class HttpPathParams
    {

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="param">Param.</param>
        public HttpPathParams(string name, string value)
    {
        Name = name;
        Value = value;
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // public properties                                                                                                //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>Gets the param name.</summary>
    public string Name
    {
        get; protected set;
    }


    /// <summary>Gets the param value.</summary>
    public string Value
    {
        get; protected set;
    }
}
}
