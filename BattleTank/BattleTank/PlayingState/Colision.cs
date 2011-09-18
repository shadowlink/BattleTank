using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TiledLib;

namespace BattleTank
{
    public class Colision
    {
        #region Members
        public Map mapa;
        TileLayer _solidos;
        public List<Player> _listaPlayers;
        private Game _game;
        private DateTime startRumble;
        private TimeSpan timeRumblePass;
        private SoundEffect _impacto;
        #endregion

        public void Initialize(Map map, Game game)
        {
            mapa = map;
            _solidos = mapa.GetLayer("Solidos") as TileLayer;
            _listaPlayers = new List<Player>();
            _game = game;
            
            //Sonidos
            _impacto = _game.Content.Load<SoundEffect>("sound/SFX/Impacto");
        }

        public void Update(Map map)
        {
            mapa = map;
            _solidos = mapa.GetLayer("Solidos") as TileLayer;
        }

        /*
         * player: jugador que hace la comprobacion, no debemos colisionar con nosotros mismos
         * pos: posicion donde comprobar la colision.
         * textura: textura del objeto del cual vamos a comprobar la colision
         * daño: daño que producira la colision en el objeto colisionado
         */ 
        public bool updateColision(Entidad entity, Player player)
        {
            bool colision = false;
            Rectangle rectangle1;
            Rectangle rectangle2;
            Color[] dataMap;
            int x = 0, y = 0;
            Vector2 entityOrigin = new Vector2(entity._texturaFrame.Width / 2, entity._texturaFrame.Height / 2);
            
            Matrix entityTransform =
            Matrix.CreateTranslation(new Vector3(-entityOrigin, 0.0f)) *
            Matrix.CreateRotationZ(-MathHelper.ToRadians(entity._anguloRotacion)) *
            Matrix.CreateTranslation(new Vector3(entity._pos, 0.0f));
            rectangle1 = CalculateBoundingRectangle(new Rectangle(0, 0, entity._texturaFrame.Width, entity._texturaFrame.Height), entityTransform);

            for (int i = 0; i < _listaPlayers.Count; i++)
            {
                if (_listaPlayers[i] != player)
                {
                    Vector2 entityOrigin2 = new Vector2(_listaPlayers[i]._texturaFrame.Width / 2, _listaPlayers[i]._texturaFrame.Height / 2);
                    Matrix entityTransform2 =
                    Matrix.CreateTranslation(new Vector3(-entityOrigin2, 0.0f)) *
                    Matrix.CreateRotationZ(-MathHelper.ToRadians(_listaPlayers[i]._anguloRotacion)) *
                    Matrix.CreateTranslation(new Vector3(_listaPlayers[i]._pos, 0.0f));
                    rectangle2 = CalculateBoundingRectangle(new Rectangle(0, 0, _listaPlayers[i]._texturaFrame.Width, _listaPlayers[i]._texturaFrame.Height), entityTransform2);

                    if(rectangle1.Intersects(rectangle2))
                    {
                        if (IntersectPixels(entityTransform, entity._texturaFrame.Width,
                                        entity._texturaFrame.Height, entity.textureData,
                                        entityTransform2, _listaPlayers[i]._texturaFrame.Width,
                                        _listaPlayers[i]._texturaFrame.Height, _listaPlayers[i].textureData))
                        {
                            _impacto.Play();
                            _listaPlayers[i]._vida -= entity._daño;
                            //_listaPlayers[i]._rumble.RumbleDisparo();
                            if (_listaPlayers[i]._vida <= 0)
                            {
                                _listaPlayers[i]._activo = false;
                                //_listaPlayers.RemoveAt(i);
                            }
                            colision = true;
                            
                        }
                    }
                }
            }

           for (x = 0; x < _solidos.Width; x++)
            {
                for (y = 0; y < _solidos.Height; y++)
                {
                    Tile tile = _solidos.Tiles[x, y];
                    if (tile != null)
                    {
                        Vector2 entityOrigin2 = new Vector2(20, 20);
                        Matrix entityTransform2 = Matrix.CreateTranslation(new Vector3(new Vector2(x*mapa.TileWidth, y*mapa.TileHeight), 0.0f));

                        rectangle2 = new Rectangle(x*mapa.TileWidth, y*mapa.TileHeight, 40,40);
                        
                        if (rectangle1.Intersects(rectangle2))
                        {
                            dataMap = ExtraerFrame(tile.Texture, tile.Source);
                            if (IntersectPixels(entityTransform, entity._texturaFrame.Width,
                                        entity._texturaFrame.Height, entity.textureData,
                                        entityTransform2, 40,
                                        40, dataMap))
                            {
                                colision = true;
                                break;
                            }

                        }
                    }
                }
            }
            return colision;
        }

        private static Color[] ExtraerFrame(Texture2D imagen, Rectangle rectImagen)
        {
            Color[] dataFrame=new Color[rectImagen.Width*rectImagen.Height];
            imagen.GetData(0, rectImagen, dataFrame, 0, rectImagen.Width * rectImagen.Height);
            return dataFrame;
        }

        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                            Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        public static bool IntersectPixels(
                    Matrix transformA, int widthA, int heightA, Color[] dataA,
                    Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                   Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}

