'use strict';

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var NameForm = function (_React$Component) {
    _inherits(NameForm, _React$Component);

    function NameForm(props) {
        _classCallCheck(this, NameForm);

        var _this = _possibleConstructorReturn(this, (NameForm.__proto__ || Object.getPrototypeOf(NameForm)).call(this, props));

        _this.state = { value: _this.props.name };

        _this.handleChange = _this.handleChange.bind(_this);
        return _this;
    }

    _createClass(NameForm, [{
        key: "handleChange",
        value: function handleChange(event) {
            this.setState({ value: event.target.value });
        }
    }, {
        key: "save",
        value: function save() {
            this.props.onSave(this.state.value);
        }
    }, {
        key: "render",
        value: function render() {
            var _this2 = this;

            return React.createElement(
                "form",
                null,
                React.createElement("input", { type: "text", value: this.state.value,
                    onChange: this.handleChange,
                    onBlur: function onBlur() {
                        return _this2.save();
                    }
                })
            );
        }
    }]);

    return NameForm;
}(React.Component);

// file or folder


var Item = function (_React$Component2) {
    _inherits(Item, _React$Component2);

    function Item(props) {
        _classCallCheck(this, Item);

        var _this3 = _possibleConstructorReturn(this, (Item.__proto__ || Object.getPrototypeOf(Item)).call(this, props));

        _this3.state = { expand: false, title: "" };
        return _this3;
    }

    _createClass(Item, [{
        key: "rename",
        value: function rename(newName) {
            if (this.props.item.title !== newName) this.props.onChangeName(newName);
        }
    }, {
        key: "delete",
        value: function _delete() {
            if (!confirm("Bạn có chắc chắn muốn xóa?")) return;

            this.props.onDelete();
        }
    }, {
        key: "renderFile",
        value: function renderFile(item) {
            var _this4 = this;

            return React.createElement(
                "span",
                { className: "text-secondary pointer" },
                React.createElement("i", { className: item.isPrivate ? "fa fa-file text-danger  mr-1" : "fa fa-file mr-1" }),
                item.title,
                React.createElement(
                    "span",
                    { className: "ml-4" },
                    React.createElement(
                        "a",
                        { className: "ml-2 text-primary", onClick: function onClick() {
                                return _this4.props.toggleShare();
                            } },
                        item.isPrivate ? "Unshare" : "Share"
                    ),
                    React.createElement(
                        "a",
                        { className: "ml-2 text-danger", onClick: function onClick() {
                                return _this4.delete();
                            } },
                        "Xo\u0301a"
                    )
                )
            );
        }
    }, {
        key: "renderFolder",
        value: function renderFolder(item) {
            var _this5 = this;

            return React.createElement(
                "span",
                { className: "text-secondary pointer" },
                React.createElement(
                    "i",
                    { className: this.state.expand ? "fa fa-minus mr-1" : "fa fa-plus  mr-1", onClick: function onClick() {
                            return _this5.toggleFolder();
                        } },
                    " "
                ),
                React.createElement("i", { className: "text-warning fa fa-folder mr-1" }),
                React.createElement(NameForm, { name: item.title, onSave: function onSave(name) {
                        return _this5.rename(name);
                    } }),
                this.state.expand ? React.createElement(ItemList, { id: item.id }) : ""
            );
        }
    }, {
        key: "toggleFolder",
        value: function toggleFolder() {
            this.setState({ expand: !this.state.expand });
        }
    }, {
        key: "render",
        value: function render() {
            var item = this.props.item;
            if (item.isFolder) {
                return this.renderFolder(item);
            } else {
                return this.renderFile(item);
            }
        }
    }]);

    return Item;
}(React.Component);

// list files


var ItemList = function (_React$Component3) {
    _inherits(ItemList, _React$Component3);

    function ItemList(props) {
        _classCallCheck(this, ItemList);

        var _this6 = _possibleConstructorReturn(this, (ItemList.__proto__ || Object.getPrototypeOf(ItemList)).call(this, props));

        _this6.state = {
            isLoaded: false,
            folders: []
        };
        return _this6;
    }

    _createClass(ItemList, [{
        key: "componentDidMount",
        value: function componentDidMount() {
            var _this7 = this;

            fetch("/api/fileManager/" + this.props.id).then(function (res) {
                return res.json();
            }).then(function (result) {
                _this7.setState({
                    isLoaded: true,
                    folders: result
                });
            }, function (error) {
                _this7.setState({
                    isLoaded: true,
                    error: error
                });
            });
        }
    }, {
        key: "renderItem",
        value: function renderItem(item) {
            var _this8 = this;

            return React.createElement(Item, { item: item,
                onDelete: function onDelete() {
                    return _this8.deleteFile(item);
                },
                toggleShare: function toggleShare() {
                    return _this8.toggleShareFile(item.id);
                },
                onChangeName: function onChangeName(name) {
                    return _this8.rename(item.id, name);
                }
            });
        }
    }, {
        key: "rename",
        value: function rename(id, name) {
            var _this9 = this;

            var requestOptions = {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: "\"" + name + "\""
            };

            fetch("/api/fileManager/" + id + "/rename", requestOptions).then(function (_) {
                var items = _this9.state.folders.map(function (item) {
                    if (item.id === id) {
                        item.title = name;
                    }

                    return item;
                });

                _this9.setState({
                    folders: items
                });
            });
        }
    }, {
        key: "deleteFile",
        value: function deleteFile(item) {
            var _this10 = this;

            var requestOptions = {
                method: 'DELETE'
            };

            fetch("/api/fileManager/" + item.id, requestOptions).then(function (result) {
                var items = _this10.state.folders.filter(function (m) {
                    return m.id !== item.id;
                });
                _this10.setState({
                    folders: items
                });
            });
        }
    }, {
        key: "toggleShareFile",
        value: function toggleShareFile(id) {
            var _this11 = this;

            var requestOptions = {
                method: 'POST'
            };

            fetch("/api/fileManager/" + id + "/toggleShare", requestOptions).then(function (_) {
                var items = _this11.state.folders.map(function (item) {
                    if (item.id === id) {
                        item.isPrivate = !item.isPrivate;
                    }

                    return item;
                });

                _this11.setState({
                    folders: items
                });
            });
        }
    }, {
        key: "handleCreateFolder",
        value: function handleCreateFolder() {
            var _this12 = this;

            var requestOptions = {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: "\"" + (this.props.id ? this.props.id : "00000000-0000-0000-0000-000000000000") + "\""
            };

            fetch("/api/fileManager/createFolder", requestOptions).then(function (res) {
                return res.json();
            }).then(function (result) {
                var items = _this12.state.folders;
                items.push(result);

                _this12.setState({
                    folders: items
                });
            });
        }
    }, {
        key: "handleUploadFile",
        value: function handleUploadFile(files) {
            var _this13 = this;

            var formData = new FormData();
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }

            if (this.props.id) {
                formData.append('parentId', this.props.id);
            }

            var requestOptions = {
                headers: { 'content-type': 'application/x-www-form-urlencoded' }
            };

            axios.post('/api/fileManager/upload', formData, requestOptions).then(function (result) {
                var items = _this13.state.folders.concat(result.data);

                _this13.setState({
                    folders: items
                });
            });
        }
    }, {
        key: "render",
        value: function render() {
            var _this14 = this;

            var _state = this.state,
                error = _state.error,
                isLoaded = _state.isLoaded,
                folders = _state.folders;

            if (error) {
                return React.createElement(
                    "div",
                    null,
                    "Error: ",
                    error.message
                );
            } else if (!isLoaded) {
                return React.createElement(
                    "div",
                    null,
                    "Loading..."
                );
            } else {
                return React.createElement(
                    React.Fragment,
                    null,
                    React.createElement(FolderAction, {
                        onCreateFolder: function onCreateFolder() {
                            return _this14.handleCreateFolder();
                        },
                        onUploadFile: function onUploadFile(files) {
                            return _this14.handleUploadFile(files);
                        }
                    }),
                    React.createElement(
                        "ul",
                        null,
                        folders.map(function (item) {
                            return React.createElement(
                                "li",
                                { key: item.id, className: "pb-1" },
                                _this14.renderItem(item)
                            );
                        })
                    )
                );
            }
        }
    }]);

    return ItemList;
}(React.Component);

var FolderAction = function (_React$Component4) {
    _inherits(FolderAction, _React$Component4);

    function FolderAction(props) {
        _classCallCheck(this, FolderAction);

        var _this15 = _possibleConstructorReturn(this, (FolderAction.__proto__ || Object.getPrototypeOf(FolderAction)).call(this, props));

        _this15.inputFileRef = React.createRef();
        return _this15;
    }

    _createClass(FolderAction, [{
        key: "onFileChange",
        value: function onFileChange(e) {
            e.preventDefault();
            this.props.onUploadFile(e.target.files);
        }
    }, {
        key: "uploadFileClick",
        value: function uploadFileClick() {
            this.inputFileRef.current.click();
        }
    }, {
        key: "render",
        value: function render() {
            var _this16 = this;

            return React.createElement(
                "p",
                null,
                React.createElement(
                    "a",
                    { href: "#", onClick: this.props.onCreateFolder, className: "mr-3 text-small" },
                    React.createElement(
                        "small",
                        null,
                        React.createElement("i", { className: "fa fa-plus-circle mr-1" }),
                        "Ta\u0323o Folder"
                    )
                ),
                React.createElement(
                    "a",
                    { href: "#", onClick: function onClick() {
                            return _this16.uploadFileClick();
                        } },
                    React.createElement("input", { ref: this.inputFileRef, style: { display: 'none' }, type: "file", multiple: true, onChange: function onChange(e) {
                            return _this16.onFileChange(e);
                        } }),
                    React.createElement(
                        "small",
                        null,
                        React.createElement("i", { className: "fa fa-upload mr-1" }),
                        "Upload"
                    )
                )
            );
        }
    }]);

    return FolderAction;
}(React.Component);

var FileManager = function (_React$Component5) {
    _inherits(FileManager, _React$Component5);

    function FileManager() {
        _classCallCheck(this, FileManager);

        return _possibleConstructorReturn(this, (FileManager.__proto__ || Object.getPrototypeOf(FileManager)).apply(this, arguments));
    }

    _createClass(FileManager, [{
        key: "showCreateFolderForm",
        value: function showCreateFolderForm() {
            console.log("showCreateFolderForm");
        }
    }, {
        key: "render",
        value: function render() {
            return React.createElement(
                React.Fragment,
                null,
                React.createElement(ItemList, { id: "00000000-0000-0000-0000-000000000000" })
            );
        }
    }]);

    return FileManager;
}(React.Component);

var domContainer = document.querySelector('#file-manager');
ReactDOM.render(React.createElement(FileManager, null), domContainer);