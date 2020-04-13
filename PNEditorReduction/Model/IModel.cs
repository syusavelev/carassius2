﻿namespace PNEditorReduction.Model
{
    public interface IModel
    {
    //    IModel GetCurrentModel();
    //    void SetCurrentModel(IModel model);
        void RedrawModel();
        void ShowCorrespondingProperties();
        void RemoveNode(Node node);
    }
}
