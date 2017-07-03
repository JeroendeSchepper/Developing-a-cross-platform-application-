﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedLogic
{
    /// <summary>
    /// Implements the IDrawable and IUpdatable for easy access.
    /// </summary>
    public interface IDrawUpdate : IDrawable, IUpdatable { }

    /// <summary>
    /// Define interfaces to allow for drawing entities and the screen.
    /// </summary>
    public interface IDrawingManager
    {
        void DrawRectangle(Point top_left_coordinate, float width, float height, Colour color);
    }
    public interface IDrawable { void Draw(IDrawVisitor visitor); }
    public interface IDrawVisitor
    {
        void DrawScreen(EntityManager entityManager);
        void DrawEntity(Entity entity);
    }
    public enum Colour { White, Hotpink };

    /// <summary>
    /// Define interfaces to allow for updating entities and screen.
    /// </summary>
    public interface IUpdateVisitor
    {
        void UpdateScreen(EntityManager entityManager, float dt);
        void UpdateEntity(Entity entity, float dt);
    }
    public interface IUpdatable { void Update(IUpdateVisitor visitor, float dt); }

    /// <summary>
    /// Concrete implementations for drawing entities on the screen.
    /// </summary>
    public class MonogameDrawingAdapter : IDrawingManager
    {
        SpriteBatch sprite_batch;
        ContentManager content_manager;

        Texture2D white_pixel;

        public MonogameDrawingAdapter(SpriteBatch sprite_batch, ContentManager content_manager)
        {
            this.sprite_batch = sprite_batch;
            this.content_manager = content_manager;
            white_pixel = content_manager.Load<Texture2D>("white_pixel");
        }

        private Microsoft.Xna.Framework.Color Convert_color(Colour color)
        {
            switch (color)
            {
                case Colour.White:
                    return Microsoft.Xna.Framework.Color.White;
                case Colour.Hotpink:
                    return Microsoft.Xna.Framework.Color.HotPink;
                default:
                    return Microsoft.Xna.Framework.Color.White;
            }
        }

        public void DrawRectangle(Point top_left_coordinate, float width, float height, Colour color)
        {
            sprite_batch.Draw(white_pixel, new Rectangle((int)top_left_coordinate.X, (int)top_left_coordinate.Y, (int)width, (int)height), Convert_color(color));
        }
    }

    /// <summary>
    /// Concrete implementation for visiting the list and drawing.
    /// </summary>
    public class DefaultDrawVisitor : IDrawVisitor
    {
        IDrawingManager drawing_manager;

        public DefaultDrawVisitor(IDrawingManager drawing_manager)
        {
            this.drawing_manager = drawing_manager;
        }

        public void DrawEntity(Entity entity)
        {
            drawing_manager.DrawRectangle(new Point(Convert.ToInt32(entity.Position.X), Convert.ToInt32(entity.Position.Y)), entity.width, entity.height, Colour.Hotpink);
        }

        public void DrawScreen(EntityManager entityManager)
        {
            entityManager.entities.Reset();
            while (entityManager.entities.GetNext().Visit(() => false, _ => true))
            {
                entityManager.entities.GetCurrent().Visit(() => { }, item => { item.Draw(this); });
            }
        }
    }
    public class DefaultUpdateVisitor : IUpdateVisitor
    {
        InputManager input_manager;
        EntityConstructor constructor;

        public DefaultUpdateVisitor(InputManager input_manager, EntityConstructor constructor)
        {
            this.input_manager = input_manager;
            this.constructor = constructor;
        }

        public void UpdateEntity(Entity entity, float dt)
        {
            entity.Checkcollision(constructor.PaddleLeft, constructor.PaddleRight);
            entity.ChangeVelocity(input_manager, dt, constructor.Ball);
            entity.CheckOutOfBounds(constructor.PaddleLeft, constructor.PaddleRight);
        }

        public void UpdateScreen(EntityManager entityManager, float dt)
        {
            entityManager.entities.Reset();
            while (entityManager.entities.GetNext().Visit(() => false, _ => true))
            {
                entityManager.entities.GetCurrent().Visit(() => { }, item => { item.Update(this, dt); });
            }
        }
    }
}