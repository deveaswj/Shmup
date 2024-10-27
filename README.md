# Shmup

From a Udemy course: [Complete C# Unity Game Developer 2D](https://www.udemy.com/course/unitycourse/)

Expanding on the original tutorial by adding:
- [x] Asteroids (background)
- Object pooling
  - [x] for projectiles
  - [ ] ~~for enemy ships~~
- Collectable power-ups (dropped randomly by defeated enemies)
  - [x] Heal: minor, major, full, increase max health
  - Ammo types
    - [x] Double-shot (bigger hitbox, double damage)
    - [x] Faster shot (single shot, but higher firing rate and velocity)
    - [x] Photons (highest damage, but smaller hitbox)
    - <details>
      <summary>(Spoilers)</summary>

      - [x] Whenever you collect the same kind of ammo that you're already using, you get a slight firing velocity boost. Stacking infinitely.
      - [ ] (TODO: alternate increasing velocity and firing rate)

    </details>
  - [x] Shield (big hitbox, takes a few hits and goes away)
  - [x] Booster (speed boost)
  - [ ] Gold Wings (increases score but also player's hitbox)
  - [x] Drones (collect up to 3; they trail the ship and fire the same projectile type)
  - [ ] 1-Up (add after implementing multiple lives)
- [x] Smoother screen shake w/Perlin noise
- [x] Enemies may Flee or Dive when damaged
- [x] Enemy classes -- color & shape determine behavior, health, firing rate, powerup drop odds
  - <details>
    <summary>(Class details)</summary>
    
    - Colors
      - Black: Standard HP, Chance to flee OR dive. Worth 100.
      - Blue: Lower HP, Chance to flee. Worth 50.
      - Green: More HP, Never flees or dives. Worth 50.
      - Orange: Standard HP, Chance to dive. Worth 75.
    - Types
      - Type 1: Prefers to drop Heal or Drone Powerups.
      - Type 2: Prefers to drop Weapon or Drone Powerups. Lower chance to Flee or Dive.
      - Type 3: Prefers to drop Shield or Heal Powerups. Higher chance to Flee or Dive.
      - Type 4: Prefers to drop Shield or Speed Powerups. Fires twice as often, and projectiles are faster.
      - Type 5: Prefers to drop Speed or Weapon Powerups.
    
  </details>
- [ ] Additional levels
  - [ ] Continue/Restart
  - [ ] Level Selection
- [ ] Boss fights
- [ ] Multiple lives

## Credits
Music: ["Mythica" by congusbongus](https://opengameart.org/users/congusbongus)

Graphics: [KennyNL](https://www.kenney.nl/assets)

(TODO: credit the star and nebula backgrounds creator)
