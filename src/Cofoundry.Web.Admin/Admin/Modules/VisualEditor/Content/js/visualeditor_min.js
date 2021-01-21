/*! Cofoundry 2020-06-22 */
angular.module("cms.visualEditor",["cms.shared"]).constant("_",window._).constant("visualEditor.modulePath","/Admin/Modules/VisualEditor/Js/"),angular.module("cms.visualEditor").factory("visualEditor.pageBlockService",["$http","shared.serviceBase","visualEditor.options",function(a,b,c){function d(a,b){return e(a)+"/"+b}function e(a){return a?h:g}var f={},g=b+"page-version-region-blocks",h=b+"custom-entity-version-page-blocks";return f.getAllBlockTypes=function(){return a.get(b+"page-block-types/")},f.getPageVersionBlockById=function(b,c){return a.get(d(b,c)+"?datatype=updatecommand")},f.getRegion=function(c){return a.get(b+"page-templates/0/regions/"+c)},f.getBlockTypeSchema=function(c){return a.get(b+"page-block-types/"+c)},f.add=function(b,d){var f=b?"customEntity":"page";return d[f+"VersionId"]=c.versionId,a.post(e(b),d)},f.update=function(b,c,e){return a.put(d(b,c),e)},f.remove=function(b,c){return a["delete"](d(b,c))},f.moveUp=function(b,c){return a.put(d(b,c)+"/move-up")},f.moveDown=function(b,c){return a.put(d(b,c)+"/move-down")},f}]),angular.module("cms.visualEditor").controller("VisualEditorController",["$window","$scope","_","shared.LoadState","shared.entityVersionModalDialogService","shared.modalDialogService","shared.localStorage","visualEditor.pageBlockService","visualEditor.modulePath","shared.urlLibrary","visualEditor.options",function(a,b,c,d,e,f,g,h,i,j,k){function l(){var b=a.addEventListener?"addEventListener":"attachEvent",c=window[b],d="attachEvent"===b?"onmessage":"message";c(d,m),D.globalLoadState=E,D.config=n,D.publish=o,D.unpublish=p,D.copyToDraft=q,D.addRegionBlock=r,D.addBlock=s,D.addBlockAbove=s,D.addBlockBelow=s,D.editBlock=t,D.moveBlockUp=u,D.moveBlockDown=u,D.deleteBlock=v}function m(a){a.data.action&&D[a.data.action]&&D[a.data.action].apply(this,a.data.args)}function n(){C={entityNameSingular:k.entityNameSingular,isCustomEntity:k.isCustomEntityRoute}}function o(a){e.publish(a.entityId,A,C).then(y)["catch"](B)}function p(a){e.unpublish(a.entityId,A,C).then(y)["catch"](B)}function q(a){e.copyToDraft(a.entityId,a.versionId,a.hasDraftVersion,A,C).then(y)["catch"](B)}function r(a){function b(){E.off()}f.show({templateUrl:i+"Routes/Modals/AddBlock.html",controller:"AddBlockController",options:{insertMode:a.insertMode,pageTemplateRegionId:a.pageTemplateRegionId,adjacentVersionBlockId:a.versionBlockId,permittedBlockTypes:a.permittedBlockTypes,isCustomEntity:a.isCustomEntity,regionName:a.regionName,pageId:a.pageId,onClose:b,refreshContent:w}})}function s(a){function b(){E.off()}E.isLoading||(E.on(),f.show({templateUrl:i+"Routes/Modals/AddBlock.html",controller:"AddBlockController",options:{pageTemplateRegionId:a.pageTemplateRegionId,adjacentVersionBlockId:a.versionBlockId,permittedBlockTypes:a.permittedBlockTypes,insertMode:a.insertMode,refreshContent:w,isCustomEntity:a.isCustomEntity,pageId:a.pageId,onClose:b}}))}function t(a){function b(){E.off()}E.isLoading||(E.on(),f.show({templateUrl:i+"Routes/Modals/EditBlock.html",controller:"EditBlockController",options:{versionBlockId:a.versionBlockId,pageBlockTypeId:a.pageBlockTypeId,isCustomEntity:a.isCustomEntity,refreshContent:w,onClose:b}}))}function u(a){var b=a.isUp?h.moveUp:h.moveDown;E.isLoading||(E.on(),b(a.isCustomEntity,a.versionBlockId).then(w)["finally"](E.off))}function v(a){function b(){return h.remove(d,a.versionBlockId).then(w)["finally"](E.off)}function c(){E.off()}var d=a.isCustomEntity,e={title:"Delete Block",message:"Are you sure you want to delete this content block?",okButtonTitle:"Yes, delete it",onOk:b,onCancel:c};E.isLoading||(E.on(),f.confirm(e))}function w(){x()}function x(){a.parent.location=a.parent.location}function y(){var b=z(a.parent.location.href,["version","mode"]);a.parent.location=b}function z(a,b){var d=a.match(/(.+)(?:\?)([^#\s]*)(#.*|)/i),e=[],f="";return d?(c.each(d[2].split("&"),function(a){if(a){var d=c.every(b,function(b){return-1===a.indexOf(b+"=")});d&&e.push(a)}}),e.length&&(f="?"+e.join("&")),a=d[1]+f+d[3]):a}function A(a){D.globalLoadState.on()}function B(a){D.globalLoadState.off()}var C,D=this,E=(a.document,new d);l()}]),angular.module("cms.visualEditor").controller("AddBlockController",["$scope","$q","_","shared.LoadState","visualEditor.pageBlockService","visualEditor.options","options","close",function(a,b,c,d,e,f,g,h){function i(){g.anchorElement;a.command={dataModel:{},pageId:g.pageId,pageTemplateRegionId:g.pageTemplateRegionId,pageVersionId:f.pageVerisonId,adjacentVersionBlockId:g.adjacentVersionBlockId,insertMode:g.insertMode||"Last"},a.submitLoadState=new d,a.formLoadState=new d(!0),a.save=k,a.close=l,a.selectBlockType=o,a.selectBlockTypeAndNext=p,a.isBlockTypeSelected=q,a.setStep=m,j()}function j(){function b(b){a.title=g.regionName,g.permittedBlockTypes.length?a.blockTypes=c.filter(b,function(a){return c.contains(g.permittedBlockTypes,a.fileName)}):a.blockTypes=b,1===a.blockTypes.length?(a.command.pageBlockTypeId=a.blockTypes[0].pageBlockTypeId,m(2)):a.allowStep1=!0,a.formLoadState.off()}m(1),e.getAllBlockTypes().then(b)}function k(){a.submitLoadState.on(),e.add(g.isCustomEntity,a.command).then(g.refreshContent).then(l)["finally"](a.submitLoadState.off)}function l(){h(),g.onClose()}function m(b){a.currentStep=b,2===b&&n()}function n(){function b(b){a.formDataSource={modelMetaData:b,model:a.command.dataModel},a.templates=b.templates,a.formLoadState.off()}a.formLoadState.on(),e.getBlockTypeSchema(a.command.pageBlockTypeId).then(b)}function o(b){a.command.pageBlockTypeId=b&&b.pageBlockTypeId}function p(a){o(a),m(2)}function q(b){return b&&b.pageBlockTypeId===a.command.pageBlockTypeId}i()}]),angular.module("cms.visualEditor").controller("EditBlockController",["$scope","$q","_","shared.LoadState","visualEditor.pageBlockService","visualEditor.options","options","close",function(a,b,c,d,e,f,g,h){function i(){g.anchorElement;a.command={dataModel:{},pageId:g.pageId},a.submitLoadState=new d,a.formLoadState=new d(!0),a.save=k,a.close=l,j()}function j(){function c(b){a.templates=b.templates,j.modelMetaData=b}function d(b){a.command=b,j.model=b.dataModel}function f(){a.formDataSource=j,a.formLoadState.off()}var h,i,j={};a.formLoadState.on(),h=e.getBlockTypeSchema(g.pageBlockTypeId).then(c),i=e.getPageVersionBlockById(g.isCustomEntity,g.versionBlockId).then(d);b.all([h,i]).then(f)}function k(){a.submitLoadState.on(),e.update(g.isCustomEntity,g.versionBlockId,a.command).then(g.refreshContent).then(l)["finally"](a.submitLoadState.off)}function l(){h(),g.onClose()}i()}]);