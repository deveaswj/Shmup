# Shmup

From a Udemy course: [Complete C# Unity Game Developer 2D](https://www.udemy.com/course/unitycourse/)

Expanding on the original tutorial by adding:
- [x] Background enhancements
  - [x] Asteroids
  - [x] "Nebula cloud" shifts randomly every 90 seconds
- [x] Energy — drains while firing, regens while not firing
- Object pooling
  - [x] for projectiles
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
      - Black: Standard HP. Worth 100. Often faster.
      - Blue: Lower HP. Worth 25. Less likely to respond when shot.
      - Green: More HP. Worth 50. Often slower.
      - Orange: Standard HP. Worth 75. More likely to respond when shot.
    - Types
      - Type 1 "Frog": Prefers to drop Heal or Drone Powerups. Likely to flee when shot.
      - Type 2 "Bat": Prefers to drop Weapon or Drone Powerups. Likely to dive when shot, or maybe flee.
      - Type 3 "Cowl": Prefers to drop Shield or Heal Powerups. Likely to flee when shot, or maybe dive.
      - Type 4 "Hex": Prefers to drop Shield or Speed Powerups. Fires twice as often, and projectiles are faster.
      - Type 5 "Vamp": Prefers to drop Speed or Weapon Powerups. Likely to dive when shot.
    
  </details>
- [ ] Additional levels
  - [ ] Continue/Restart
  - [ ] Level Selection
- [x] Boss fights
- [ ] Multiple lives? (But we already get so many Health drops. Maybe instead "retries" where you keep your score on replay?)

## Credits
Music: ["Mythica" by congusbongus](https://opengameart.org/users/congusbongus)

Graphics: [KenneyNL](https://www.kenney.nl/assets)

(TODO: credit the star and nebula backgrounds creator)
