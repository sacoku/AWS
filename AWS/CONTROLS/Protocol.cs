﻿//-----------------------------------------------------------------------
// <copyright file="Protocol.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace AWS.MODEL
{
    /// <summary>
    /// Protocol을 관리하는 클래스
    /// </summary>
    public class Protocol : System.ComponentModel.Component
    {
        /// <summary>
        ///  프로토콜을 가지는 리스트 
        /// </summary>
        private System.Collections.ArrayList m_ProtocolItems;

        /// <summary>
        /// 리스트의 Property
        /// </summary>
        public System.Collections.ArrayList ProtocolItems
        {
            get { return this.m_ProtocolItems; }
        }

        /// <summary>
        /// 프로토콜 리스트 갯수
        /// </summary>
        public int ProtocolItemCount
        {
            get { return this.m_ProtocolItems.Count; }
        }

        public Protocol()
        {
            m_ProtocolItems = new System.Collections.ArrayList();
        }

        public bool AddProtocolItem(int nBufferSize, bool bUsing, CheckFunction CheckFunc, CatchFunction CatchFunc)
        {
            ProtocolItem pI = new ProtocolItem();
            pI.Queue.SetSize(nBufferSize);
            pI.Using = bUsing;
            pI.OnCheck = CheckFunc;
            pI.OnCatch = CatchFunc;

            this.m_ProtocolItems.Add(pI);

            return true;
        }

        public void ProtocolProcessing(byte data)
        {
            try
            {
                for (int count = 0; count < this.m_ProtocolItems.Count; count++)
                {
                    if (this.m_ProtocolItems[count] is ProtocolItem == true)
                    {
                        ProtocolItem pI = (ProtocolItem)this.m_ProtocolItems[count];
                        pI.ProcessingData(data);
                    }
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
        }
    }
}
