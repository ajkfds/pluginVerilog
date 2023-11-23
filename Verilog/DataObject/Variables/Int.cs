﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Int : IntegerAtomVariable
    {
        protected Int() { }

        public static new Int Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Int);
            DataTypes.IntegerAtomType dType = dataType as DataTypes.IntegerAtomType;

            Int val = new Int();
            val.DataType = dType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Int val = new Int();
            val.DataType = DataType;
            val.Signed = Signed;
            return val;
        }


    }
}
