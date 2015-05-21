using System;
using System.Collections.Generic;
using OrbitOne.BuildScreen.Models;

namespace OrbitOne.BuildScreen.Converters
{
    public interface IConverter<U> 
    {
        List<BuildInfoDto> Convert(U result);
    }
}