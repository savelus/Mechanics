namespace Data {
    public interface IGameStateProvider {
        void SaveGameState();
        void LoadGameState();
    }
}