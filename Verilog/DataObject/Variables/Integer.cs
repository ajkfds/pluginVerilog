﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Integer : IntegerAtomVariable
    {
        protected Integer() { }

        public static new Integer Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Int);
            DataTypes.IntegerAtomType dType = dataType as DataTypes.IntegerAtomType;

            Integer val = new Integer();
            val.DataType = dType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Integer val = new Integer();
            val.DataType = DataType;
            val.Signed = Signed;
            return val;
        }

    }
}
