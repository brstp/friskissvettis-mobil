
// global storage for child items
var selectChildren = {};

// helper function to setup a relationship between two selects
function setupParentChildSelect(config) {

    var $parentSelect = $(config.parentSelector);
    var $childSelect = $(config.childSelector);

    // load main items 
    $.ajax({
        type: "GET",
        url: config.url,
        dataType: "xml",
        success: function (xmlDoc) {

            var options = '';
            selectChildren[config.storageKey] = [];

            $(xmlDoc).find(config.parentItem).each(function () {

                var id = $(this).find(config.id + ":first").text();
                var name = $(this).find(config.name + ":first").text();

                options += getOption(id, name);

                // get child data
                $(this).find(config.childItem).each(function () {

                    selectChildren[config.storageKey].push({
                        id: $(this).find(config.id).text(),
                        fk_parentId: id,
                        name: $(this).find(config.name).text()
                    });
                });
            });

            $parentSelect.html(options);

            // select parent select default value
            $parentSelect.val(config.selectedParent);

            // reload child select
            updateChild();
        }
    });

    // reload child when parent changes
    $parentSelect.change(function () {
        updateChild();
    });

    function updateChild() {

        // get selected parent
        var selectedValue = $parentSelect.val();

        // add children based on top select
        var options = '';

        // clear select
        $childSelect.html("");

        var children = selectChildren[config.storageKey];
        var counter = 0;

        $.each(children, function (index, item) {

            if (item.fk_parentId == selectedValue) {
                options += getOption(item.id, item.name);
                counter++;
            }
        });

        var $childContainer = $(config.childContainerSelector);
        var $childLabel = $(config.childLabelSelector);

        if (counter == 0) {
            $childContainer.hide();
            $childLabel.hide();
        } else {
            $childContainer.show();
            $childLabel.show();
        } 

        // add new options to select
        $childSelect.html(options);

        // select parent select default value
        $childSelect.val(config.selectedChild);
    }
}

// helper functions 
function getOption(id, name) {
    return '<option value="' + id + '">' + name + '</option>';
}