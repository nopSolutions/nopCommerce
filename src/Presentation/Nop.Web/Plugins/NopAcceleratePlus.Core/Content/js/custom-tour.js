/* globals hopscotch: false */

/* ============ */
/* EXAMPLE TOUR */
/* ============ */
var tour = {
    id: 'hello-hopscotch',
    steps: [
        {
            target: 'DownloadZip',
            title: 'Setup Files Download',
            content: 'This will gives you files you will need to setup a remote Solr instance (follow the Setup Wizard instructions for the same). However, if you Solr instance is hosted locally, we will do setup it automatically.',
            placement: 'left',
            yOffset: -15,
            xOffset: -15
        },
        {
            target: 'WizardSetupStep',
            title: 'Goto Wizard Setup',
            content: 'This will start the Setup Wizard which will guide you to for setting up the Plugin.',
            placement: 'left',
            yOffset: -15,
            xOffset: 15
        },
        {
            target: 'ExportSetting',
            placement: 'left',
            title: 'Export settings',
            content: 'Use this to export plugin settings. This will export a XML file that you can use to import the settings again.',
            yOffset: -15,
            xOffset: 15
        },
        {
            target: 'ImportSetting',
            placement: 'left',
            title: 'Import settings',
            content: 'Now you can save your time by importing the plugin settings you have already configured on staging site or from some other site where you have already configured it.',
            yOffset: -15,
            xOffset: 15
        },
        {
            target: 'CheckSolrConnection',
            placement: 'right',
            title: 'Check Solr Connection',
            content: 'This checks connection between your Site and Solr Core.',
            yOffset: -15,
            xOffset: -15
        },
        {
            target: 'Reload',
            placement: 'right',
            title: 'Reload',
            content: 'This reloads the Solr Core using a Solr Reload API. Reload core will apply any changes you have done recently to the Solr Configs. Note that, some configuration options, such as the dataDir location and IndexWriter-related settings in solrconfig.xml can not be changed and made active with a simple RELOAD action.',
            yOffset: -15,
            xOffset: -15
        },
        {
            target: 'StartIndexing',
            placement: 'right',
            title: 'Start Indexing',
            content: 'Solr needs data to be indexed before it can return requests. Do not worry. We got you covered.The Plugin, tracks what entities has changed and does indexing in the background which is triggered by the Scheduled Task.Moreover, when you have Indexing on Entity Change set to enabled, the Plugin will detect any change to entity you do from Administration Panel and does real time indexing to Solr.And if you want to start indexing manually, use this button.',
            yOffset: -15,
            xOffset: -15
        },
        {
            target: 'StopIndexing',
            placement: 'right',
            title: 'Stop Indexing',
            content: 'Sometimes you want to stop indexing for some reason in-between. Use this to stop indexing if its already running.',
            yOffset: -15,
            xOffset: -15
        },
        {
            target: 'ReIndexing',
            placement: 'right',
            title: 'Re-Indexing',
            content: 'Using ReIndex feature lets you re-build your Solr index completely from scratch. This will delete existing index data and mark all products as pending for re-indexing as well as start indexing again. This comes handy when troubleshooting indexing related issue.',
            yOffset: -15,
            xOffset: -15
        },
        {
            target: 'MarkAllAsPending',
            placement: 'left',
            title: 'Mark All As Pending',
            content: 'This will mark all entities index status as Pending. That means, it will index all the entities again overwriting existing index data. Again, this is helpful at the time of troubleshooting.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'ClearCoreData',
            placement: 'left',
            title: 'Clear Index Data',
            content: 'You can use this to delete all the data indexed into Solr. This will clear everything indexed and you will need to perform full indexing again.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'ManageIncrementalTable',
            placement: 'left',
            title: 'Recreate Status Table',
            content: 'This will recreate the table (NopAcc_Plus_Incremental_Solr_Product) which tracks the entities status for indexation. Once done, it will be in a state of when plugin was installed. This is helpful for troubleshooting purposes. Do not use this until you understand what you&#39;re doing.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'SolrCoreURL',
            placement: 'left',
            title: 'Solr Core URL',
            content: 'Stores Solr Core URL. Solr Core URL is different then Solr URL. For example my solr server is at 127.0.0.1:8983/solr but my core is actually at 127.0.0.1:8983/solr/my_core Solr Core URL is required by Plugin to talk to Solr and performs all operations including configurations, indexing & querying (fetching data) from Solr.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'PatternDelimited',
            placement: 'left',
            title: 'Pattern Delimited',
            content: 'Specifies Pattern Delimiter &#39;SplitBy&#39; Attribute used during Indexing. You should not change this unless advised by nopAccelerate Support team.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'BatchSize',
            placement: 'left',
            title: 'Indexing BatchSize',
            content: 'Indexing Process fetches data from SQL Database and index them into Solr in required format. Indexing is costly and therefore indexing for your catalog is done in smaller batches. This setting defines size of such batch. A default value is good enough, however you can experiment with different value. Indexing is done automatically in the background. A full indexing is required only for the first time setup or when there are any changes into Solr&#39;s Schema. Look into Release note for instruction for the same.',
            yOffset: -15,
            xOffset: 10,
            width: 400
        },
        {
            target: 'DeleteBatchSize',
            placement: 'left',
            title: 'Delete BatchSize',
            content: 'Just like indexing, we often need to delete old data from Solr Index to maintain the integrity of the index.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'ClearCacheTime',
            placement: 'left',
            title: 'Clear CacheTime',
            content: 'Using this you can control the cache time for the plugins.',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'AdvanceSearchEnable',
            placement: 'left',
            title: 'Advance Search Handler',
            content: '<p>Its possible that you may have some custom boosting requirement which may not be possible with boosting settings offered by Plugin. You may use Advance Search Handler in such cases and configure custom boosting on fields you want to search inside Solr Config file. This removes requirement for customizing the code which may be costly and not recommended. Configuring this feature is not part of standard support. Please <a style="font-weight:bold;" href="https://www.nopaccelerate.com/contact/" target="_blank">contact our Sales team</a> for getting paid consulting & support on the same if you want to implement Advance Search handler to customize your search relevancy & boost settings.</p>',
            yOffset: -25,
            xOffset: 10,
            width: 400
        },
        {
            target: 'PerformIndexingOnEntityChange',
            placement: 'left',
            title: 'Perform Indexing on Entity Change',
            content: '<p>If this setting is enable, then on any entity change, it will automatic perform indexing for pending products</p><p>We recommend to disable this if you are bulk updating catalog entities using third party / external tools / script, as this tracks nopCommerce entity change event and starts indexing in the background which can cause issue when youre performing bulk update.<p><p>In such cases, we recommend to index using scheduled task or by doing a get request to "yourwebsite.com/Admin/NopAcceleratePlusSearch/StartIndexing" to start indexing manually after your bulk update process. Please check <a href="http://docs.nopaccelerate.com/files/nopaccelerate-plus-plug-in/ImplementWebRequestforStartIndex.html" target="_blank">documentation</a> for start indexing using get request</p>',
            yOffset: -25,
            xOffset: 10,
            width: 400
        },
        {
            target: 'EmailNotification',
            placement: 'left',
            title: 'Email Notification',
            content: 'Sometimes your Solr instance may get down or unreachable. It&#39;s very rare, however if you configured your email address here; you will receive an email notification alerts when Solr is unreachable.<bold>NOTE:</bold>It will send email to first email address and other comma separated EmailIds are in CC',
            yOffset: -15,
            xOffset: 10
        },
        {
            target: 'StoreSolrQuery',
            placement: 'left',
            title: 'Enable Solr Debug Query',
            content: 'This is only for developer purpose...If you want to store the SolrQuery in Solr log then enable this setting',
            yOffset: -25,
            xOffset: 10
        },
    ],
    showPrevButton: true,
    scrollTopMargin: 100
},

    /* ========== */
    /* TOUR SETUP */
    /* ========== */
    addClickListener = function (el, fn) {
        if (el.addEventListener) {
            el.addEventListener('click', fn, false);
        }
        else {
            el.attachEvent('onclick', fn);
        }
    },

    init = function () {
        var startBtnId = 'startTourBtn',
            calloutId = 'startTourCallout',
            mgr = hopscotch.getCalloutManager(),
            state = hopscotch.getState();

        if (state && state.indexOf('hello-hopscotch:') === 0) {
            // Already started the tour at some point!
            hopscotch.startTour(tour);
        }
        else {
            // Looking at the page for the first(?) time.
            if ($('#ShowTourTipOnFirstLoad').is(":checked")) {
                setTimeout(function () {
                    mgr.createCallout({
                        id: calloutId,
                        target: startBtnId,
                        placement: 'right',
                        title: 'Take a tour',
                        content: 'Start by taking a tour to get more information about nopAccelerate plus configuration!',
                        yOffset: -25,
                        arrowOffset: 20,
                        width: 240
                    });
                }, 100);
            }
        }

        addClickListener(document.getElementById(startBtnId), function () {
            if (!hopscotch.isActive) {
                mgr.removeAllCallouts();
                hopscotch.startTour(tour);
            }
        });
    };

init();