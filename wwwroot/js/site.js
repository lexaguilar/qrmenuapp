// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const processResponse = resp => {

    return new Promise((resolve, reject) => {

        if (resp.status == 400)
            resp.text().then(err => reject(err));


        if (resp.status == 200)
            resp.json().then(data => resolve(data))

    })

}

const http = url => {

    let base = (url, properties) => {
        return new Promise((resolve, reject) => {

            fetch(`${url}`, properties)
                .then(processResponse)
                .catch(error => reject(error))
                .then(response => resolve(response));

        })
    };

    let getParameters = function(data) {
        let queryString = '';

        if (data) {
            let parameters = '';

            if (typeof data == 'function')
                parameters = data();
            else
                parameters = data;

            for (const prop in parameters) {
                queryString += `&${prop}=${parameters[prop]}`
            }

        }

        return queryString.length > 0 ? queryString.replace('&', '?') : queryString;

    };


    const _url = `${path}${url}`;

    return {

        asGet: (data = null) => {

            let params = getParameters(data);

            return base(`${_url}${params}`, { method: 'GET' });

        },
        asPost: data => {
            return new Promise((resolve, reject) => {
                fetch(_url, {
                        method: 'POST',
                        body: data ? JSON.stringify(data) : null,
                        headers: {
                            "Content-Type": "application/json;charset=UTF-8"
                        }
                    })
                    .then(processResponse)
                    .catch(error => reject(error))
                    .then(response => resolve(response));
            })
        },
        asDelete: (data = null) => {
            let params = getParameters(data);

            return base(`${_url}${params}`, { method: 'DELETE' });

        },
        asFile: (file = null) => {
            return new Promise(resolve => {
                let formData = new FormData();
                formData.append('file', file);
                fetch(_url, {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(response => resolve(response))
                    .catch(error => console.error('Error:', error));
            })
        },
    }
}


$(function(){
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
            ].forEach(function(i) {
                if (i in loadOptions && isNotEmpty(loadOptions[i]))
                    args[i] = JSON.stringify(loadOptions[i]);
            });
            $.ajax({
                url: path + "api/menu/get",
                dataType: "json",
                data: args,
                success: function(result) {
                    deferred.resolve(result.items, {
                        totalCount: result.totalCount,
                    });
                },
                error: function() {
                    deferred.reject("Data Loading Error");
                },
                timeout: 5000
            });

            return deferred.promise();
        },
        insert: (data) => {
            return new Promise(resolve =>
                http("api/menu/post").asPost(data).then(result => {
                    resolve(result);
                })
            )
        },
        update: (data, dataModificada) => {

            return new Promise(resolve =>
                http("api/menu/put").asPost({...data, ...dataModificada }).then(result => {
                    resolve(result);
                })

            )
        }
    });

    $("#gridContainer").dxDataGrid({
        dataSource: store,
        showBorders: true,
        remoteOperations: true,
        paging: {
            pageSize: 12
        },
        editing: {
            mode: "cell",
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true
        }, 
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [8, 12, 20]
        },
        columns: [{
            width: 300,
            type: "buttons",
            width: 110,
            buttons: ['edit',{
                hint: "Clone",
                icon: "repeat",
                onClick: function(e) {
                    console.log(e.row.data.name);
                   window.open(`/home/privacy?name=${e.row.data.name}` , '_blank');
                }
            }],
        },{
            dataField: "name",
            visible: false
        }, {
            dataField: "descripcionName",
            validationRules: [
                { type: "required" },
                {
                    type: "stringLength",
                    message: "Min 2 carateres y maximo 150",
                    max : 150,
                    min : 2
                }
            ]
        }]
    }).dxDataGrid("instance");
});
