﻿using System.Collections.Generic;
using System.Drawing;
using Barbar.HexGrid;

namespace qwerty.Objects
{
    public enum ObjectType
    {
        Ship,
        Meteor
    }

    public abstract class SpaceObject
    {
        public readonly int MaxActions;

        public readonly int MaxHealth;

        // TODO: is this field really needed?
        public readonly ObjectType objectType;

        public readonly Player Owner;

        public List<PointF> PolygonPoints;

        protected SpaceObject(Player owner, int maxHealth, ObjectType objectType, int maxActions)
        {
            this.Owner = owner;
            this.MaxHealth = maxHealth;
            this.CurrentHealth = maxHealth;
            this.objectType = objectType;
            this.MaxActions = maxActions;
            this.ActionsLeft = maxActions;
        }

        public int ActionsLeft { get; set; }
        public int CurrentHealth { get; set; }
        public OffsetCoordinates ObjectCoordinates { get; set; }
        public abstract string Description { get; }
        public bool IsMoving { get; set; }

        public abstract void Rotate(double angle);
    }
}