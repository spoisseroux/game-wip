public interface IDamageable {
    void TakeDamage(int amount); // NEED TO ADD SOURCE RESOLUTION!!!
    void TakeDamage(int amount, IHitboxSource source);
}