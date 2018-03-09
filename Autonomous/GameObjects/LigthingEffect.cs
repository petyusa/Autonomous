using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous.Impl.GameObjects
{
    public class LigthingEffect
    {
        public LigthingEffect()
        {
        }

        public void Apply(BasicEffect effect)
        {
            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(.6f, 0.6f, 0.6f);
            var v = new Vector3(2f, -2f, -2f);
            v.Normalize();
            effect.DirectionalLight0.Direction = v;
            effect.DirectionalLight0.SpecularColor = new Vector3(0.4f, 0.4f, 0.4f);

            //effect.DirectionalLight1.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
            //v = new Vector3(2f, -2f, 100f);
            //v.Normalize();
            //effect.DirectionalLight1.Direction = v;
            //effect.DirectionalLight1.SpecularColor = new Vector3(0.1f, 0.03f, 0.2f);
            
            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);

//            effect.FogStart = 0;
//            effect.FogEnd = 100;
        }
    }
}
