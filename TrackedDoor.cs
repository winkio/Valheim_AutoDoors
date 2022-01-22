using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoDoors
{
    public class TrackedDoor
    {
        public int Id { get; private set; }
        public ZNetView ZNetView { get; private set; }
        public int State { get; private set; }
        public bool IsValid { get; private set; } = true;

        public bool InAutoRange { get; set; }
        public bool IsManual { get; set; }
        public bool IsAutoOpened { get; set; }

        public TrackedDoor(int id, ZNetView zNetView)
        {
            Id = id;
            ZNetView = zNetView;
            Update();
            IsManual = State != 0;
        }

        public bool Update()
        {
            var obj = UnityEngine.Object.FindObjectFromInstanceID(Id);
            if (obj != null && obj is Door d)
            {
                var zd0 = ZNetView?.GetZDO();
                if (zd0 != null)
                {
                    IsValid = true;
                    State = ZNetView.GetZDO().GetInt("state", 0);
                }
                else
                    IsValid = false;
            }
            else
            {
                IsValid = false;
            }

            return IsValid;
        }

        public void SetState(int newState)
        {
            var zd0 = ZNetView?.GetZDO();
            if (zd0 != null)
                zd0.Set("state", newState);
        }
    }
}
