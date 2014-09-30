// load data from server when ready
$(document).ready(function () {

    // helper to get querystring
    // http://stackoverflow.com/questions/901115/get-query-string-values-in-javascript
    function getQueryStringParameterByName(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.search);
        if (results == null)
            return "";
        else
            return decodeURIComponent(results[1].replace(/\+/g, " "));
    }


    // setup select parent/child config
    var config = {

        // parent xml url
        url: 'handler/ActivityHandler.ashx?facilityId=' + getQueryStringParameterByName("facilityId") + "&localizationLanguage=" + localizationLanguage,

        // name in global child cache 
        storageKey: 'base',

        // name of item tag
        parentItem: "item",

        // name of item tag
        childItem: "child",

        // id tag
        id: "id",

        // name tag
        name: "name",

        // parent id tag
        parentId: "fk_parentId",

        // parent selector
        parentSelector: "[id$='drpActivity']",

        // child selector
        childSelector: "[id$='drpActivityTypes']",

        // select container selector (for hiding when no children)
        childContainerSelector: "[id$='divActivityTypes']",

        // select label selector (for hiding when no children)
        childLabelSelector: "[id$='lblActivityTypes']",

        // selected value for parent
        selectedParent: selectedActivityId,

        // selected value for child
        selectedChild: selectedActivityChildId
    }

    // sets up the parent/child relation between two selects
    setupParentChildSelect(config);

    // setup date-selects 
    var $fromSelect = $("[id$='drpFrom']");
    var $toSelect = $("[id$='drpTo']");

    $fromSelect.change(function () {
        updateToSelect();
    });

    updateToSelect();

    // updates the "to datetime" select
    function updateToSelect() {

        var fromSelectedIndex = $fromSelect.find('option:selected').index();
        var toSelectedValue = $toSelect.find('option:selected').val();
        var options = "";

        $fromSelect.find("option").each(function () {

            var currentIndex = $(this).index();

            if (currentIndex >= fromSelectedIndex) {

                var value = $(this).attr("value");
                var text = $(this).text().trim()

                options += getOption(value, text);
            }
        });

        $toSelect.html(options);

        // restore selection
        $toSelect.val(toSelectedValue);
    }
}); 