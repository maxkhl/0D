using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
/*
namespace ODBank.GameObjects
{
   /// <summary>
   /// Emits specified particles a certain way. Use this to access the particle system.
   /// </summary>
   class ParticleEmitter : DrawableObject
   {
       /// <summary>
       /// Gets a list of all living particles.
       /// </summary>
       public List<Particle> Particles { get; private set; }

       /// <summary>
       /// Gets or sets settings for newly spawned particles
       /// </summary>
       public ParticleSettings ParticleDefaultSettings { get; set; }

       /// <summary>
       /// Gets or sets a value indicating whether this <see cref="ParticleEmitter"/> is automaticaly emitting particles.
       /// </summary>
       public bool Emission { get; set; }

       /// <summary>
       /// Gets or sets the minimum amount of particles to emit per second.
       /// </summary>
       public int EmissionMin { get; set; }

       /// <summary>
       /// Gets or sets the maximum amount of particles to emit per second.
       /// </summary>
       public int EmissionMax { get; set; }

       /// <summary>
       /// Random instance used by this emitter and all of its particles
       /// </summary>
       public Random Random { get; set; }

       /// <summary>
       /// Position that specifies where particles spawn
       /// </summary>
       public Vector2 Position { get; set; }

       public SpriteObject AttachedTo { get; set; }
       public Vector2 LocalPosition { get; set; }

       public ParticleEmitter(Scene GameScene) : base(GameScene)
       {
           Particles = new List<Particle>();
           Random = new Random();
       }


       private float SecondTick = 0;
       private int EmissionTick = 0;
       public override void Update(GameTime gameTime)
       {
           this.Position = AttachedTo.Position + LocalPosition;

           if (this.Emission)
           {
               var EmissionsPerSecond = EmissionMax - EmissionMin;
               if (EmissionsPerSecond < 0)
                   throw new Exception("Emitters minimum emission is bigger than its maximum emission value");

               SecondTick += gameTime.ElapsedGameTime.Milliseconds;

               var EmissionsThatShouldHaveHappenedTillNowBitch = (int)(SecondTick / 1000 * EmissionsPerSecond);

               var MissingEmissions = EmissionsThatShouldHaveHappenedTillNowBitch - EmissionTick;

               if(MissingEmissions > 0)
               {
                   Emit(MissingEmissions);
                   EmissionTick += MissingEmissions;
               }

           }

           for (int i = 0; i < Particles.Count; i++)
           {
               Particles[i].Update(gameTime);
           }

           base.Update(gameTime);
       }

       public override void Draw(GameTime gameTime)
       {
           for (int i = 0; i < Particles.Count; i++)
               Particles[i].Draw(gameTime);

           base.Draw(gameTime);
       }

       /// <summary>
       /// Emits a specified amount of particles.
       /// </summary>
       /// <param name="Amount">The amount of particles to emit.</param>
       public void Emit(int Amount)
       {
           for(int i = 0; i < Amount; i++)
           {
               Particles.Add(new Particle(this, this.ParticleDefaultSettings));
           }
       }

       /// <summary>
       /// Removes all particles that are alive right now.
       /// </summary>
       public void ClearParticles()
       {
           Particles.Clear();
       }

       public struct ParticleSettings
       {
           /// <summary>
           /// The particles texture
           /// </summary>
           public hTexture ParticleTexture;

           public Color ParticleColor;

           public TextureDirections TextureDirection;
           public enum TextureDirections
           {
               FollowRotation,
               FollowDirection,
           }

           public string AnimationSequence;
           public bool AnimationRepeat;

           /// <summary>
           /// The angular velocity of the particle
           /// </summary>
           public float AngularVelocity;

           public bool RndAngularVelocity;
           public float RndAngularVelocityMin;
           public float RndAngularVelocityMax;

           /// <summary>
           /// The linear velocity of the particle
           /// </summary>
           public Vector2 LinearVelocity;

           public bool RndLinearVelocity;
           public Vector2 RndLinearVelocityMin;
           public Vector2 RndLinearVelocityMax;


           /// <summary>
           /// Lifetime of the particle in MS
           /// </summary>
           public float Lifetime;

           public bool RndLifetime;
           public float RndLifetimeMin;
           public float RndLifetimeMax;


           public float LinearDamping;
           public float AngularDamping;
       }

       public class Particle : IDisposable
       {
           public ParticleEmitter Emitter { get; set; }

           public float TotalLifetime { get; set; }
           public float Lifetime { get; set; }

           public hTexture Texture { get; set; }

           public Color Color { get; set; }

           private ParticleSettings.TextureDirections TextureDirection { get; set; }

           public Particle(ParticleEmitter Emitter, ParticleSettings Settings)
           {
               this.TextureDirection = Settings.TextureDirection;


               this.Emitter = Emitter;




               if (Settings.ParticleTexture != null && Settings.ParticleTexture.IsAnimation)
               {
                   this.Texture = Settings.ParticleTexture.Copy(); // Animations need to be copied in order to get individual animations
                   if (Settings.AnimationSequence == null)
                       this.Texture.Play(Settings.AnimationRepeat);
                   else
                       this.Texture.Play(Settings.AnimationRepeat, Settings.AnimationSequence);
               }
               else
               {
                   this.Texture = Settings.ParticleTexture;
               }



               this.Color = Settings.ParticleColor;

               if(Settings.RndLifetime)
                   TotalLifetime = 
                       ((float)Emitter.Random.NextDouble() * (Settings.RndLifetimeMax - Settings.RndLifetimeMin)) + Settings.RndLifetimeMin;
               else
                   TotalLifetime = Settings.Lifetime;

               this.Lifetime = 0;

              this.Body = BodyFactory.CreateRectangle(
                   Emitter.PhysicEngine.World, 
                   ConvertUnits.ToSimUnits(0.5f), 
                   ConvertUnits.ToSimUnits(0.5f), 
                   ConvertUnits.ToSimUnits(1f), 
                   this);

               this.Body.IsStatic = false;
               //this.Body.CollidesWith = FarseerPhysics.Dynamics.Category.StaticGOs;

               // If emitter attached to physical object
               if (Emitter.AttachedTo != null && typeof(BodyObject).IsAssignableFrom(Emitter.AttachedTo.GetType()))
                   this.Body.IgnoreCollisionWith(((BodyObject)Emitter.AttachedTo).Body);

               this.Body.CollisionCategories = FarseerPhysics.Dynamics.Category.Particles;
                
               this.Body.Restitution = 0.1f;
               this.Body.Friction = 0.1f;
               this.Body.Position = ConvertUnits.ToSimUnits(Emitter.Position);

               this.Body.LinearDamping = Settings.LinearDamping;
               this.Body.AngularDamping = Settings.AngularDamping;

               if (Settings.RndAngularVelocity)
                   this.Body.AngularVelocity =
                       ConvertUnits.ToSimUnits(((float)Emitter.Random.NextDouble() * (Settings.RndAngularVelocityMax - Settings.RndAngularVelocityMin)) + Settings.RndAngularVelocityMin);
               else
                   this.Body.AngularVelocity = Settings.AngularVelocity;

               if (Settings.RndLinearVelocity)
                   this.Body.LinearVelocity = ConvertUnits.ToSimUnits(new Vector2(
                       ((float)Emitter.Random.NextDouble() * (Settings.RndLinearVelocityMax.X - Settings.RndLinearVelocityMin.X)) + Settings.RndLinearVelocityMin.X,
                       ((float)Emitter.Random.NextDouble() * (Settings.RndLinearVelocityMax.Y - Settings.RndLinearVelocityMin.Y)) + Settings.RndLinearVelocityMin.Y));
               else
                   this.Body.LinearVelocity = Settings.LinearVelocity;
           }

           public void Update(GameTime gameTime)
           {
               this.Lifetime += gameTime.ElapsedGameTime.Milliseconds;

               if (this.Lifetime > this.TotalLifetime)
                   this.Dispose();
           }

           public void Draw(GameTime gameTime)
           {                
               if(this.Texture != null)
                   this.Texture.Draw(gameTime, 
                       new Rectangle(
                           (int)ConvertUnits.ToDisplayUnits(this.Body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(this.Body.Position.Y),
                           this.Texture.Width,
                           this.Texture.Height),
                       this.Color,
                       TextureDirection == ParticleSettings.TextureDirections.FollowDirection ?
                       (float)Math.Atan2(this.Body.LinearVelocity.Y, this.Body.LinearVelocity.X) :
                       this.Body.Rotation,                     
                       new Vector2(this.Texture.Width, this.Texture.Height) / 2,
                       SpriteEffects.None, 0);
           }

           public void Collision(FarseerPhysics.Dynamics.Body Object, Vector2 Position)
           {
               this.Dispose();
           }

           public bool Disposing { get; private set; }
           public void Dispose()
           {
               this.Body.Dispose();
               this.Emitter.Particles.Remove(this);
               this.Texture.Dispose(false);
               this.Disposing = true;
           }
       }
   }
}
*/