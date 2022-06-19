using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/2/2019 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine.Assets.Entities
{
    public class Actor : DynamicEntity
    {
        public string ActorName { get; set; }

        public bool IsPlayer { get; set; }
        public Actor()
        {
            this.ActorName = "actor.essential";
            this.IsPlayer = false;
        }
    }
}
