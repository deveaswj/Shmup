# Shmup

From a Udemy course: [Complete C# Unity Game Developer 2D](https://www.udemy.com/course/unitycourse/)

Expanding on the original tutorial by adding:
- [x] Asteroids (background)
- Object pooling
  - [x] for projectiles
  - [ ] ~~for enemy ships~~
- Collectable power-ups (dropped randomly by defeated enemies)
  - [x] Heal: minor, major, full, increase max health
  - Alternative projectiles
    - [x] Double-shot (bigger hitbox)
    - [x] Faster shot (higher firing rate and speed)
    - [x] Photons (high-damage but smaller hitbox)
  - [x] Shield (big hitbox, takes a few hits and goes away)
  - [x] Booster (speed boost)
  - [ ] Gold Wings (increases score but also player's hitbox)
  - [x] Drones (collect up to 3; they trail the ship and fire the same projectile type)
- [x] Smoother screen shake w/Perlin noise
- [x] Enemies may Flee or Dive when damaged
- [x] Enemy classes -- color & shape determine behavior, health, firing rate, powerup drop odds
