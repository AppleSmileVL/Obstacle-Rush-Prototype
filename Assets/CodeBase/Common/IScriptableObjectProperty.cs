using UnityEngine;

interface IScriptableObjectProperty // Ётот интерфейс определ€ет контракт дл€ классов, которые могут примен€ть свойства из ScriptableObject.
                                    // ќн содержит один метод ApplyProperty, который принимает ScriptableObject в качестве параметра.
                                    //  лассы, реализующие этот интерфейс, должны предоставить реализацию метода ApplyProperty дл€ применени€ свойств из переданного ScriptableObject.
{
    void ApplyProperty(ScriptableObject property);
}
