using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DisplayMessageAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}
