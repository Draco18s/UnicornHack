﻿using UnicornHack.Utils.MessagingECS;

namespace UnicornHack.Systems.Knowledge
{
    public class XPGainedMessage : IMessage
    {
        private GameEntity _entity;

        public GameEntity Entity
        {
            get => _entity;
            set
            {
                _entity?.RemoveReference(this);
                _entity = value;
                _entity?.AddReference(this);
            }
        }

        public int ExperiencePoints { get; set; }

        string IMessage.MessageName { get; set; }

        public void Dispose()
        {
            Entity = default;
            ExperiencePoints = default;
        }
    }
}
