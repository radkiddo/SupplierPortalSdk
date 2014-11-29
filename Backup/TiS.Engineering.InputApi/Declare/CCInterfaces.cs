using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TiS.Core.eFlowAPI;
using TiS.Engineering.InputApi;
using System.Xml.Serialization;

namespace TiS.Engineering.InputApi
{
    #region Class interfaces
    /// <summary>
    /// Name string property.
    /// </summary>
    interface ICCName { String Name { get; set; } }

    /// <summary>
    /// String lock etxnsion property
    /// </summary>
    interface ICCLockExtension { String LockExtension { get; set; } }

    /// <summary>
    /// Flowtype property
    /// </summary>
    interface ICCFlowType { String FlowType { get; set; } }

    /// <summary>
    /// Copy Source files property
    /// </summary>
    interface ICCCopySourceFiles { bool CopySourceFiles { get; set; } }

    /// <summary>
    /// CCParent property.
    /// </summary>
    interface ICCParent { CCEflowBaseObject CCParent { get; set; } }

    /// <summary>
    /// Parent creator property
    /// </summary>
    interface ICCParentCreator { CCreator ParentCreator { get; set; } }

    /// <summary>
    /// CCField array interface
    /// </summary>
    interface ICCFieldArray : ICCUserTags, ICCSpecialTags, ICCNamedUserTags, ICCParent, ICCLinkedFields, ICCParentCollection,
                ICCParentForm, ICCParentPage, ICCParentGroup, ICCParentTable, ICCGetField
#if !INTERNAL
         ,ICCParentCreator
#endif
    { }


    /// <summary>
    /// IRect interface
    /// </summary>
    interface IRect:IDimensions,IPosition{}

    /// <summary>
    /// CCField rect interface
    /// </summary>
    interface ICCFieldRect : IRect, IRectExt { }

    /// <summary>
    /// Attachments property interface
    /// </summary>
    interface ICCAttachments { String[] Attachments { get; set; } }

    /// <summary>
    /// Named parent property interface
    /// </summary>
    interface ICCNamedParent { String NamedParent { get; set; } }

    /// <summary>
    /// Contents property interface
    /// </summary>
    interface ICCContents { String Contents { get; set; } }

    /// <summary>
    /// Bottom property interface
    /// </summary>
    interface IBottom { int Bottom { get; set; } }

    /// <summary>
    /// Extended rect properties interface (Bottom and Right)
    /// </summary>
    interface IRectExt : IBottom, IRight { }

    /// <summary>
    /// Height property interface
    /// </summary>
    interface IHeight {  int Height { get; set; }}

    /// <summary>
    /// Left property interface
    /// </summary>
    interface ILeft {    int Left { get; set; } }

    /// <summary>
    /// Top property interface
    /// </summary>
    interface ITop {  int Top { get; set; } }

    /// <summary>
    /// Width property interface
    /// </summary>
    interface IWidth    {   int Width { get; set; }}

    /// <summary>
    /// Position property interface
    /// </summary>
    interface IPosition:ILeft,ITop { }

    /// <summary>
    /// Dimensions property interface
    /// </summary>
    interface IDimensions : IWidth, IHeight { }

    /// <summary>
    /// Right property interface
    /// </summary>
    interface IRight { int Right { get; set; } }       

    /// <summary>
    /// Rectangle property interface
    /// </summary>
    interface IRectangle { CollectionOcrData.OcrRect Rectangle { get; set; } }

    /// <summary>
    /// Image path property interface.
    /// </summary>
    interface ICCImagePath { String ImagePath { get; set; } }

    interface ICCCollection : ICCForms, ICCPages, ICCLinkedGroups, ICCAttachments, ICCLinkedFields,
        ICCLinkedTables, ICCImagePath, ICCName, ICCNamedUserTags, ICCUserTags, ICCSpecialTags,
        ICCLoginApplication, ICCLoginStation, ICCFlowType, ICCCopySourceFiles, IUpdateExistingCollection,
        ITargetQueue, IPriorityLevel, IToXml { }

    interface ICCEflowOwner { ITisDataLayerTreeNode EflowOwner { get; set; } }

    interface IToXml { bool ToXml(String xmlPath);}

    interface IFromXml { object FromXml(String xmlFilePath);}

    interface IToXmlString  { String ToXmlString();}

    interface IFromXmlString { object FromXmlString(String xmlFileContents);}

    interface IGetConfiguration { CCConfiguration.CCConfigurationData GetConfiguration(String configName);   }            

    interface ICCIndex { int Index { get; set; } }

    interface IUpdateExistingCollection { bool UpdateExistingCollection { get; set; } }

    interface IPriorityLevel { WorkflowPriorityLevel Priority { get; set; } }

    interface ISystemRect { System.Drawing.Rectangle Rect { get; set; } }

    interface ITargetQueue { String TargetQueue { get; set; } }

    interface ICCUseSourceNamedUserTags { bool UseSourceNamedUserTags { get; } }

    interface ICCPageID { String PageID { get; set; } }

    interface ICCBackPage { bool BackPage { get; set; } }

    interface ICCFrontPage { bool FrontPage { get; set; } }

    interface ICCFormType { String FormType { get; set; } }

    interface ICCLoginApplication { String LoginApplication { get; set; } }

    interface ICCLoginStation { String LoginStation { get; set; } }

    interface ICCParentPage { ITisPageData ParentPage { get; set; } }

    interface ICCParentForm { ITisFormData ParentForm { get; set; } }

    interface ICCParentTable { ITisFieldTableData ParentTable { get; set; } }

    interface ICCParentFieldArray { ITisFieldArrayData ParentFieldArray { get; set; } }

    interface ICCParentGroup { ITisFieldGroupData ParentGroup { get; set; } }

    interface ICCParentCollection { ITisCollectionData ParentCollection { get; set; } }

    interface ICCLinkedFields { CCCollection.CCField[] LinkedFields { get; } }

    interface ICCFieldArrays { CCCollection.CCFieldArray[] FieldArrays { get; } }

    interface ICCTable : ICCFieldArrays, ICCParent, ICCParentGroup, ICCParentCollection,
                    ICCParentForm, ICCUserTags, ICCSpecialTags, ICCNamedUserTags, ICCGetField 
#if !INTERNAL
        , ICCParentCreator
#endif
    { }

    interface ICCLinkedGroups { CCCollection.CCGroup[] LinkedGroups { get; } }

    interface ICCLinkedTables { CCCollection.CCTable[] LinkedTables { get; } }

    interface ICCGroup : ICCParent, ICCParentCollection, ICCParentForm, ICCParentPage,
                 ICCLinkedFields, ICCLinkedTables, ICCNamedUserTags, ICCUserTags, ICCSpecialTags, ICCGetField, ICCGetTable
#if !INTERNAL
        ,ICCParentCreator
#endif
    { }

    interface ICCForm : ICCParentCollection, ICCParent, ICCParentCreator, ICCLinkedFields, ICCFormType, ICCPages,
                ICCLinkedGroups, ICCLinkedTables, ICCSpecialTags, ICCNamedUserTags, ICCUserTags, ICCGetField, ICCGetGroup, ICCGetTable { }

    interface ICCPages { CCCollection.CCPage[] Pages { get; } }

    interface ICCForms { CCCollection.CCForm[] Forms { get; } }

    interface ICCEnabled { bool Enabled { get; set; } }

    interface IKey { String Key { get; set; } }

    interface IVal { String Val { get; set; } }

    interface IIndex { int Index { get; set; } }

    interface IXmlPath { String XmlPath { get; set; } }

    interface ICharValue { Char Value { get; set; } }

    interface IConfidence { int Confidence { get; set; } }

    interface IHasRect { CCCollection.FieldRect Rect { get; set; } }

    interface ICreator { CCreator CollectionsCreator { get; } }

    interface ICSM { ITisClientServicesModule CSM { get; set; } }

    interface ICollectionID { String CollectionID  { get; set; } }

    interface ISearchHandler { CCSearchFiles SearchHandler { get; set; } }

    interface ICCDict : IKey, IVal { }

    interface ICCSpecialTags { CCCollection.CCDictContainer SpecialTags { get; set; } }

    interface ICCNamedUserTags { CCCollection.CCDictContainer NamedUserTags { get; set; } }

    interface ICCUserTags { CCCollection.CCDictContainer UserTags { get; set; } }

    interface ICCLastErrors { String LastErrors { get; } }

    interface IProfileNames { String[] ProfileNames { get; } }
     
    interface ILinesSeparator {   String LinesSeparator { get; set; } }

    interface IWordsSeparator { String WordsSeparator { get; set; } }

    interface IPrdFilePath { String PrdFilePath { get; set; } }

    interface IPrdStyle { TiS.Recognition.Common.TWord.TStyle Style { get; set; } }

    interface IPrdPages { CollectionOcrData.PageOcrData[] Pages { get; set; } }

    interface IPrdLines { CollectionOcrData.LineOcrData[] Lines { get; set; } }

    interface IPrdWords { CollectionOcrData.WordOcrData[] Words { get; set; } }

    interface IPrdChars { CollectionOcrData.CharOcrData[] Chars { get; set; } }

    interface IConfigurations { CCConfiguration.CCConfigurationData[] Configurations { get; set; } }

    interface IFilesSort { int FilesSort { get; set; } }

    interface ICopySourceFiles { bool CopySourceFiles { get; set; } }

    interface IErrorFolderPath { String ErrorFolderPath { get; set; } }

    interface IIgnoreExceptions { bool IgnoreExceptions { get; set; } }

    interface IIgnoreNamedUserTags { bool IgnoreNamedUserTags { get; set; } }

    interface IIgnoreUserTags { bool IgnoreUserTags { get; set; } }

    interface IMaxCsmCount { int MaxCsmCount { get; set; } }

    interface IAttachmentsExtensions { String[] AttachmentsExtensions { get; set; } }
    
    interface IKeepFileName { bool KeepFileName { get; set; } }
    
    interface IMaxFilesLock { int MaxFilesLock { get; set; } }
    
    interface IMultiPagePerForm { bool MultiPagePerForm { get; set; } }
    
    interface ISearchExtensions {   String[]   SearchExtensions { get; set; } }
    
    interface ISearchPaths {   String[]   SearchPaths { get; set; } }
    
    interface ISearchSubFolders { bool SearchSubFolders { get; set; } }
    
    interface ITimerIntervals { int TimerIntervals { get; set; } }

    interface IThrowAllExceptions { bool ThrowAllExceptions { get; set; } }

    interface IXmlFilePath { String XmlFilePath { get; set; } }

    interface IErrorFolderDateFormat { String ErrorFolderDateFormat { get; set; } }

    interface ICCValue {  String Value { get; } }

    interface ICCCurrentCollection { CCCollection CurrentCollection { get; set; } }

    interface ICCEflowObject : ICCName, ICCUserTags, ICCNamedUserTags, ICCSpecialTags, ICCEflowOwner, ICCNamedParent { }

    interface ICCEflowBaseObject : ICCName, ICCNamedUserTags, ICCUserTags, ICCEflowOwner, ICCNamedParent, ICCParentCollection, ICCParent
#if !INTERNAL   
        ,ICCParentCreator 
#endif
    { }

    interface ICCCretaor : IDisposable, ICCCurrentBaseProfile, ICCCurrentCollection, ICCLastErrors { }

    interface ICCCurrentBaseProfile { CCConfiguration.CCBaseConfigurationData CurrentProfile { get; set; } }

    interface ICCCurrentProfile { CCConfiguration.CCConfigurationData CurrentProfile { get; set; } }

    interface ICCSearchFiles : IDisposable, ICCCurrentProfile { }

    interface ICCField : ICCContents, ICCParentGroup, ICCParentPage, ICCParentForm, ICCParentTable, ICCParentFieldArray, ICCIndex
                , ICCParent, ICCParentCollection, ICCNamedUserTags, ICCSpecialTags, ICCUserTags, IConfidence, IHasRect 
#if !INTERNAL
        , ICCParentCreator
#endif
    { }

    interface ICCPage : ICCParent, ICCParentCollection, ICCParentForm, ICCLinkedFields, ICCPageID, ICCLinkedGroups,
                                ICCAttachments, ICCLinkedTables, ICCFrontPage, ICCBackPage, ICCNamedUserTags, ICCUserTags, ICCGetField, ICCGetGroup, ICCGetTable
#if !INTERNAL
        , ICCParentCreator
#endif
    { }

    interface ICCTimerSearch : IDisposable, ICCCurrentProfile, ICCEnabled, ICreator, ICSM, ISearchHandler { }

    interface IPageOcrData : ICCGenericSerializer, IIndex, ICCValue, ILinesSeparator, IPrdFilePath, IPrdLines { }

    interface ILineOcrData : IIndex, ICCValue, IWordsSeparator, IPrdWords { }

    interface IWordOcrData : IPrdChars, IIndex, ICCValue, IConfidence, IRectangle, IPrdStyle { }

    interface ICharOcrData : IOcrData, ISystemRect, ICharValue { }

    /// <summary>
    /// IOcrData interface
    /// </summary>
    interface IOcrData : ICCGenericSerializer, IConfidence, IIndex, IRectangle { }

    interface ICollectionOcrData : ICCGenericSerializer, ICCFlowType, IPrdPages, ICollectionID { }

    interface ICCGenericSerializer : IFromXml, IToXml, IToXmlString, IFromXmlString, ICloneable { }

    //interface ISetParents { void SetParents(); }

    interface ICCGetField { CCCollection.CCField GetField(String fieldName); }    

    interface ICCGetGroup { CCCollection.CCGroup GetGroup(String groupName); }

    interface ICCGetTable { CCCollection.CCTable GetTable(String tableName); }

    interface ICCBaseConfigurationData : ICCName, ICCLockExtension, ICCUseSourceNamedUserTags, ICCLoginApplication, 
                ICCLoginStation, IXmlFilePath, ICopySourceFiles, IIgnoreExceptions, IIgnoreNamedUserTags, IIgnoreUserTags, 
                IMaxCsmCount, IThrowAllExceptions, IErrorFolderPath, IErrorFolderDateFormat { }

    interface ICCConfiguration : IXmlPath, IProfileNames, IConfigurations, IGetConfiguration { }

    interface ICCConfigurationData : ICCBaseConfigurationData, IFilesSort, IAttachmentsExtensions, IKeepFileName,
                IMaxFilesLock, IMultiPagePerForm, ISearchExtensions, ISearchPaths, ISearchSubFolders, ITimerIntervals { }
    #endregion
}