

$(function () {
    let empresa;
    var empresaNameArr = window.location.href.split('/');
    var empresaName = empresaNameArr[empresaNameArr.length - 1];
    empresa = empresaName.split('=')[1];

    function isNotEmpty(value) {
        return value !== undefined && value !== null && value !== "";
    }
    var store = new DevExpress.data.CustomStore({
        load: function (loadOptions) {
            var deferred = $.Deferred(),
                args = {};

            [
                "skip",
                "take",
            ].forEach(function (i) {
                if (i in loadOptions && isNotEmpty(loadOptions[i]))
                    args[i] = JSON.stringify(loadOptions[i]);
            });



            $.ajax({
                url: path + "api/item/all?empresaName=" + empresa,
                dataType: "json",
                data: args,
                success: function (result) {
                    deferred.resolve(result.items, {
                        totalCount: result.totalCount,
                    });
                },
                error: function () {
                    deferred.reject("Data Loading Error");
                },
                timeout: 5000
            });

            return deferred.promise();
        },
        insert: (data) => {
            return new Promise(resolve =>
                http("api/item/post").asPost({ ...data, empresaName: empresa }).then(result => {
                    resolve(result);
                })
            )
        },
        update: (data, dataModificada) => {

            return new Promise(resolve =>
                http("api/item/put").asPost({ ...data, ...dataModificada }).then(result => {
                    resolve(result);
                })

            )
        },
        remove: catalogo => {
            return new Promise(resolve =>
                http(model.uri.remove(catalogo.id)).asGet().then(result => {
                    notify(model.msgDeleted, 'error');
                    resolve(result);
                })
            )
        },
    });

    $("#gridContainer2").dxDataGrid({
        dataSource: store,
        showBorders: true,
        remoteOperations: true,
        paging: {
            pageSize: 12
        },
        editing: {
            mode: "row",
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [8, 12, 20]
        },
        columns: [{
            dataField: "title",
        }, {
            dataField: "categoriaId",
            lookup: {
                dataSource: new DevExpress.data.CustomStore({
                    key: "id",
                    loadMode: "raw",
                    load: function () {
                        return new Promise(resolve => {
                            http(`api/categorias`)
                                .asGet()
                                .then(r => resolve(r))
                        });
                    }
                }),
                displayExpr: "descripcion",
                valueExpr: "id"
            }
        }, {
            dataField: "monedaId",
            lookup: {
                dataSource: new DevExpress.data.CustomStore({
                    key: "id",
                    loadMode: "raw",
                    load: function () {
                        return new Promise(resolve => {
                            http(`api/monedas`)
                                .asGet()
                                .then(r => resolve(r))
                        });
                    }
                }),
                displayExpr: "descripcion",
                valueExpr: "id"
            }
        }, {
            dataField: "price",
            dataType: 'number'

        }, {
            dataField: "isSuggestion",
            dataType: 'boolean'
        }, {
            dataField: "descripcion",
        }, {
            dataField: "urlImagen",
        }]
    }).dxDataGrid("instance");
});