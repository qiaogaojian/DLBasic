public interface IUnirxFSM<in T>
{
    string state { get; }

    void OnEnter(T t);

    void OnUpdate(T t);

    void OnExit(T t);
}