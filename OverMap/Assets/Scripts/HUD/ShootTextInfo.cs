using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    public class ShootTextInfo
    {
        public string content;
        public TextAnimationType animationType;
        public TextMoveType moveType;
        public float delayMoveTime;
        public int size;
        public Transform cacheTranform;
        public float initializedVerticalPositionOffset;
        public float initializedHorizontalPositionOffset;
        public float xIncrement;
        public float yIncrement;
    }
}
