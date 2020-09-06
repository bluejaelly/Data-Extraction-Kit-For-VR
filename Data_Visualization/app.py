import pandas as pd
import math
import numpy as np
import pathlib
from pathlib import Path
import dash
import datetime as dt
from datetime import datetime
from dash.dependencies import Input, Output, State, ClientsideFunction, MATCH, ALL, ALLSMALLER
import dash_core_components as dcc
import dash_html_components as html
import dash_table
import plotly.express as px
import plotly.graph_objects as go
from plotly.subplots import make_subplots
import glob
import os
import json
import io

app = dash.Dash(
    name = __name__,
    assets_folder = "assets",
    meta_tags= [{"name": "viewport", "content": "width=device-width"}]
)

app.layout = html.Div(
    [
        dcc.Store(id="aggregate_data"),
        html.Div(id="output-clientside"),

        # Header
        html.Div(
            [
                html.Div(
                    html.H3(
                        "Data Extraction Kit for VR",
                        style={"marginBottom": "25px", "textAlign": "center"}
                    ),
                    id="header",
                    className = "row flex_display",
                    style={"marginBottom": "25px"}
                ),

                # Tabs(project selection)
                html.Div(
                    [
                        dcc.Tabs(id="tabs", className="custom-tabs", children=[
                            dcc.Tab(label='Head Gaze', value='gaze', className="custom-tab", style={"backgroundColor":"#e6e6e6"}, children=[
                                html.Div(
                                    [
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                dcc.Graph()
                                            ],
                                            className = "row flex_display",
                                        )
                                    ]
                                )  
                            ]),
                            dcc.Tab(label='Controller Point', value='point', className="custom-tab", style={"backgroundColor":"#e6e6e6"},children=[
                                html.Div(
                                    [
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        )
                                    ]
                                )  
                            ]),
                            dcc.Tab(label='Controller Grab', value='grab', className="custom-tab", style={"backgroundColor":"#e6e6e6"},children=[
                                html.Div(
                                    [
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        )
                                    ]
                                )  
                            ]),
                            dcc.Tab(label='Player Position', value='position', className="custom-tab", style={"backgroundColor":"#e6e6e6"}, children=[
                                dcc.Graph()
                            ]),
                        ]),
                        html.Div(id='cur-tab', style={'display':'none'})
                    ],
                    id="tabs-container",
                    className="row flex_display",
                ),
            ]
        )
    ]
)





server = app.server

if __name__ == "__main__":
    app.run_server(debug = True)