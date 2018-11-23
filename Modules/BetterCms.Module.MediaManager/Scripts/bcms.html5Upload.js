﻿/*jslint unparam: true, white: true, browser: true, devel: true */
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="bcms.html5Upload.js" company="Devbridge Group LLC">
// 
// Copyright (C) 2015,2016 Devbridge Group LLC
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
// 
// <summary>
// Better CMS is a publishing focused and developer friendly .NET open source CMS.
// 
// Website: https://www.bettercms.com 
// GitHub: https://github.com/devbridge/bettercms
// Email: info@bettercms.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------

bettercms.define('bcms.html5Upload', ['bcms.jquery', 'bcms'], function ($, bcms) {
    'use strict';

    var module = {},
        noop = function () { },
        supportsFileApi;

    // Upload manager constructor:
    function UploadManager(options) {
        var self = this;
        self.dropContainer = options.dropContainer;
        self.inputField = options.inputField;
        self.uploadsQueue = [];
        self.activeUploads = 0;
        self.data = options.data;
        self.key = options.key;
        self.maxSimultaneousUploads = options.maxSimultaneousUploads || -1;
        self.onFileAdded = options.onFileAdded || noop;
        self.uploadUrl = options.uploadUrl;
        self.onFileAddedProxy = function (upload) {
            bcms.logger.trace('Event: onFileAdded, file: ' + upload.fileName);
            self.onFileAdded(upload);
        };

        self.initialize();
    }

    // FileUpload proxy class:
    function FileUpload(file) {
        var self = this;

        self.file = file;
        self.fileName = file.name;
        self.fileSize = file.size;
        self.uploadSize = file.size;
        self.uploadedBytes = 0;
        self.eventHandlers = {};
        self.abort = function () {
            bcms.logger.warn('Abort impossible. File upload not initialized for file ' + self.fileName);
        };
        self.events = {
            onProgress: function (fileSize, uploadedBytes) {
                var progress = uploadedBytes / fileSize * 100;
                bcms.logger.trace('Event: upload onProgress, progress = ' + progress + ', fileSize = ' + fileSize + ', uploadedBytes = ' + uploadedBytes);
                (self.eventHandlers.onProgress || noop)(progress, fileSize, uploadedBytes);
            },
            onStart: function () {
                bcms.logger.trace('Event: upload onStart');
                (self.eventHandlers.onStart || noop)();
            },
            onCompleted: function (data) {
                bcms.logger.trace('Event: upload onCompleted, data = ' + data);
                file = null;
                (self.eventHandlers.onCompleted || noop)(data);
            },
            onError: function (status, response) {
                bcms.logger.trace('Event: upload onError (status code ' + status + ')');
                file = null;
                (self.eventHandlers.onError || noop)();
            },
            onTransfer: function (transferProg) {
                if (transferProg >= 100) {
                    transferProg = 0;
                } else {
                    transferProg += 10;
                }
                (self.eventHandlers.onTransfer || noop)(transferProg);
            }
        };
    }

    FileUpload.prototype = {
        on: function (eventHandlers) {
            this.eventHandlers = eventHandlers;
        }
    };

    UploadManager.prototype = {

        initialize: function () {
            bcms.logger.debug('Initializing upload manager');
            var manager = this,
                dropContainer = manager.dropContainer,
                inputField = manager.inputField,
                cancelEvent = function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                };

            if (dropContainer) {
                manager.on(dropContainer, 'dragover', cancelEvent);
                manager.on(dropContainer, 'dragenter', cancelEvent);
                manager.on(dropContainer, 'drop', function (e) {
                    cancelEvent(e);
                    manager.processFiles(e.dataTransfer.files);
                });
            }

            if (inputField) {
                manager.on(inputField, 'change', function () {
                    manager.processFiles(this.files);
                    $(this).val('');
                });
            }
        },

        processFiles: function (files) {
            bcms.logger.trace('Processing files: ' + files.length);
            var manager = this,
                len = files.length,
                file,
                upload,
                i;

            for (i = 0; i < len; i += 1) {
                file = files[i];
                if (file.size === 0) {
                    alert('Files with files size zero cannot be uploaded or multiple file uploads are not supported by your browser');
                    break;
                }

                upload = new FileUpload(file);
                manager.uploadFile(upload);
            }
        },

        uploadFile: function (upload) {
            var manager = this;

            manager.onFileAdded(upload);

            // Queue upload if maximum simultaneous uploads reached:
            if (manager.activeUploads === manager.maxSimultaneousUploads) {
                bcms.logger.trace('Queue upload: ' + upload.fileName);
                manager.uploadsQueue.push(upload);
                return;
            }

            manager.ajaxUpload(upload);
        },

        ajaxUpload: function (upload) {
            var manager = this,
                xhr,
                formData,
                fileName,
                file = upload.file,
                prop,
                data = manager.data,
                key = manager.key || 'file';

            bcms.logger.trace('Begin upload: ' + upload.fileName);
            manager.activeUploads += 1;

            xhr = new window.XMLHttpRequest();
            formData = new window.FormData();
            fileName = file.name;

            xhr.open('POST', manager.uploadUrl);

            // Triggered when upload starts:
            xhr.upload.onloadstart = function () {
                // File size is not reported during start!
                bcms.logger.trace('Upload started: ' + fileName);
                upload.events.onStart();
            };

            // Triggered many times during upload:
            xhr.upload.onprogress = function (event) {
                if (!event.lengthComputable) {
                    return;
                }

                // Update file size because it might be bigger than reported by the fileSize:
                upload.events.onProgress(event.total, event.loaded);
                if (event.total == event.loaded) {
                    upload.events.onTransfer();
                }
            };

            // Triggered when upload is completed:
            xhr.onload = function (event) {
                bcms.logger.trace('Upload completed: ' + fileName);

                // Reduce number of active uploads:
                manager.activeUploads -= 1;
                if (event.target.status === 200) {
                    upload.events.onCompleted(event.target.responseText);
                } else {
                    upload.events.onError(event.target.status, event.target.responseText);
                }

                // Check if there are any uploads left in a queue:
                if (manager.uploadsQueue.length) {
                    manager.ajaxUpload(manager.uploadsQueue.shift());
                }
            };

            // Triggered when upload fails:
            xhr.onerror = function (event) {
                bcms.logger.error('Upload failed: ', upload.fileName);
                upload.events.onError(event.target.status, event.target.responseText);
            };

            // Append additional data if provided:
            if (data) {
                for (prop in data) {
                    if (data.hasOwnProperty(prop)) {
                        bcms.logger.trace('Adding data: ' + prop + ' = ' + data[prop]);
                        formData.append(prop, data[prop]);
                    }
                }
            }

            // Append file data:
            formData.append(key, file);

            // Initiate upload:
            xhr.send(formData);

            upload.abort = function () {
                bcms.logger.trace('Upload aborted for ' + upload.fileName);
                xhr.abort();
                manager.activeUploads -= 1;
                if (manager.uploadsQueue.length) {
                    manager.ajaxUpload(manager.uploadsQueue.shift());
                }
            };
        },

        // Event handlers:
        on: function (element, eventName, handler) {
            if (!element) {
                return;
            }
            if (element.addEventListener) {
                element.addEventListener(eventName, handler, false);
            } else if (element.attachEvent) {
                element.attachEvent('on' + eventName, handler);
            } else {
                element['on' + eventName] = handler;
            }
        }
    };

    module.fileApiSupported = function () {
        if (typeof supportsFileApi !== 'boolean') {
            var input = document.createElement("input");
            input.setAttribute("type", "file");
            supportsFileApi = !!input.files;
        }

        return supportsFileApi;
    };

    module.initialize = function (options) {
        return new UploadManager(options);
    };

    return module;
});