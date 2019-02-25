using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Messages
{
    /// <summary>
    /// Type specific patch target
    /// link an action to a resource and create a resourceAction
    /// op:add path:actionid value:12312432-12312-12312-3123
    /// to unlink an action from a resource by resouceactionId
    /// op:remove path:resourceactionid value 543543-53452123-3123-31254
    /// </summary>
    public class ResourcePatch
    {
        public string[] ActionId { get; set; }
        public string[] DeleteResourceActionId { get; set; }
    }
}
    