﻿using EFWCoreLib.WcfFrame.SDMessageHeader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using ProtoBuf.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFWCoreLib.WcfFrame.DataSerialize
{
    /// <summary>
    /// 服务输出数据
    /// </summary>
    public class ServiceResponseData
    {
        List<string> _listjson;
        public ServiceResponseData()
        {
            _listjson = new List<string>();
        }

        public ServiceResponseData(bool IsCompress, bool IsEncrytion, SerializeType SerializeType)
        {
            _iscompressjson = IsCompress;
            _isencryptionjson = IsEncrytion;
            _serializetype = SerializeType;

            _listjson = new List<string>();
        }

        #region wcf服务配置属性
        bool _iscompressjson = false;
        bool _isencryptionjson = false;
        SerializeType _serializetype = SerializeType.Newtonsoft;

        public bool Iscompressjson
        {
            get
            {
                return _iscompressjson;
            }

            set
            {
                _iscompressjson = value;
            }
        }

        public bool Isencryptionjson
        {
            get
            {
                return _isencryptionjson;
            }

            set
            {
                _isencryptionjson = value;
            }
        }

        public SerializeType Serializetype
        {
            get
            {
                return _serializetype;
            }

            set
            {
                _serializetype = value;
            }
        }
        #endregion

        /// <summary>
        /// 添加输出数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void AddData<T>(T data)
        {
            if (_serializetype == SerializeType.Newtonsoft)
            {
                if (data is DataTable)
                {
                    _listjson.Add(JsonConvert.SerializeObject(data, Formatting.Indented));
                }
                else
                {
                    _listjson.Add(JsonConvert.SerializeObject(data));
                }
            }
            else if (_serializetype == SerializeType.protobuf)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (data is DataTable)
                    {
                        object obj = data;
                        DataSerializer.Serialize(ms, (DataTable)obj);
                    }
                    else
                    {
                        Serializer.Serialize(ms, data);
                    }
                    _listjson.Add(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
            else if (_serializetype == SerializeType.fastJSON)
            {
                _listjson.Add(fastJSON.JSON.ToJSON(data));
            }
        }

        /// <summary>
        /// 获取输出的指定数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T GetData<T>(int index)
        {
            if (_serializetype == SerializeType.Newtonsoft)
            {
                return JsonConvert.DeserializeObject<T>(_listjson[index]);
            }
            else if (_serializetype == SerializeType.protobuf)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(_listjson[index]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    if (default(T) is DataTable)
                    {
                        Object obj = DataSerializer.DeserializeDataTable(ms);
                        return (T)obj;
                    }
                    else
                    {
                        return Serializer.Deserialize<T>(ms);
                    }
                }
            }
            else if (_serializetype == SerializeType.fastJSON)
            {
                return fastJSON.JSON.ToObject<T>(_listjson[index]);
            }
            else
                return default(T);
        }

        /// <summary>
        /// 获取输出的Json数据
        /// </summary>
        /// <returns></returns>
        public string GetJsonData()
        {
            return JsonConvert.SerializeObject(_listjson);
        }
        /// <summary>
        /// 设置输出的Json数据
        /// </summary>
        /// <param name="retData"></param>
        public void SetJsonData(string retData)
        {
            if (string.IsNullOrEmpty(retData) == false)
                _listjson = JsonConvert.DeserializeObject<List<string>>(retData);
        }
    }
}
