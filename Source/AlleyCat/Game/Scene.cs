using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Game
{
    public class Scene : GameObject, IScene
    {
        public string Key { get; }

        public Node Root { get; }

        public Node CharactersRoot => _charactersPath.Bind(Root.FindComponent<Node>).IfNone(Root);

        public Node ItemsRoot => _itemsPath.Bind(Root.FindComponent<Node>).IfNone(Root);

        public Node UIRoot => _uiPath.Bind(Root.FindComponent<Node>).IfNone(Root);

        private Option<NodePath> _charactersPath;

        private Option<NodePath> _itemsPath;

        private Option<NodePath> _uiPath;

        public Scene(
            string key,
            Node root,
            Option<NodePath> charactersPath,
            Option<NodePath> itemsPath,
            Option<NodePath> uiPath)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(root, nameof(root)).IsNotNull();

            Key = key;
            Root = root;

            _charactersPath = charactersPath;
            _itemsPath = itemsPath;
            _uiPath = uiPath;
        }

        public PackedScene Pack() => throw new NotImplementedException();
    }
}
