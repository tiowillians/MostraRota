using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.Interfaces
{
    public interface IImageResizer
    {
        byte[] ResizeImage(byte[] imageData, float width, float height);
        byte[] LoadFromResource(string resourceName);
    }
}
